/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
==================================================================== */

namespace NPOI.XSSF.UserModel
{


    using NPOI.SS.UserModel;
    using NPOI.Util;
    using NPOI.OpenXmlFormats.Dml;
    using System.IO;
    using NPOI.OpenXml4Net.OPC;
    using System;
    using NPOI.OpenXmlFormats.Spreadsheet;
    using System.Drawing;


    /**
     * Represents a picture shape in a SpreadsheetML Drawing.
     *
     * @author Yegor Kozlov
     */
    public class XSSFPicture : XSSFShape, IPicture
    {
        private static POILogger logger = POILogFactory.GetLogger(typeof(XSSFPicture));

        /**
         * Column width measured as the number of characters of the maximum digit width of the
         * numbers 0, 1, 2, ..., 9 as rendered in the normal style's font. There are 4 pixels of margin
         * pAdding (two on each side), plus 1 pixel pAdding for the gridlines.
         *
         * This value is the same for default font in Office 2007 (Calibry) and Office 2003 and earlier (Arial)
         */
        private static float DEFAULT_COLUMN_WIDTH = 9.140625f;

        /**
         * A default instance of CTShape used for creating new shapes.
         */
        private static CT_Picture prototype = null;

        /**
         * This object specifies a picture object and all its properties
         */
        private CT_Picture ctPicture;

        /**
         * Construct a new XSSFPicture object. This constructor is called from
         *  {@link XSSFDrawing#CreatePicture(XSSFClientAnchor, int)}
         *
         * @param Drawing the XSSFDrawing that owns this picture
         */
        protected XSSFPicture(XSSFDrawing drawing, CT_Picture ctPicture)
        {
            this.drawing = drawing;
            this.ctPicture = ctPicture;
        }

        /**
         * Returns a prototype that is used to construct new shapes
         *
         * @return a prototype that is used to construct new shapes
         */
        internal static CT_Picture Prototype()
        {
            if (prototype == null)
            {
                CT_Picture pic = CT_Picture.Factory.newInstance();
                CT_PictureNonVisual nvpr = pic.AddNewNvPicPr();
                CT_NonVisualDrawingProps nvProps = nvpr.AddNewCNvPr();
                nvProps.id = (1);
                nvProps.name = ("Picture 1");
                nvProps.descr = ("Picture");
                CT_NonVisualPictureProperties nvPicProps = nvpr.AddNewCNvPicPr();
                nvPicProps.AddNewPicLocks().SetNoChangeAspect(true);

                CT_BlipFillProperties blip = pic.AddNewBlipFill();
                blip.AddNewBlip().SetEmbed("");
                blip.AddNewStretch().AddNewFillRect();

                CT_ShapeProperties sppr = pic.AddNewSpPr();
                CT_Transform2D t2d = sppr.AddNewXfrm();
                CT_PositiveSize2D ext = t2d.AddNewExt();
                //should be original picture width and height expressed in EMUs
                ext.cx = (0);
                ext.cy = (0);

                CT_Point2D off = t2d.AddNewOff();
                off.x=(0);
                off.y=(0);

                CT_PresetGeometry2D prstGeom = sppr.AddNewPrstGeom();
                prstGeom.prst = (ST_ShapeType.rect);
                prstGeom.AddNewAvLst();

                prototype = pic;
            }
            return prototype;
        }

        /**
         * Link this shape with the picture data
         *
         * @param rel relationship referring the picture data
         */
        protected void SetPictureReference(PackageRelationship rel)
        {
            ctPicture.blipFill.blip.embed = rel.Id;
        }

        /**
         * Return the underlying CT_Picture bean that holds all properties for this picture
         *
         * @return the underlying CT_Picture bean
         */

        public CT_Picture GetCTPicture()
        {
            return ctPicture;
        }

        /**
         * Reset the image to the original size.
         *
         * <p>
         * Please note, that this method works correctly only for workbooks
         * with the default font size (Calibri 11pt for .xlsx).
         * If the default font is Changed the resized image can be streched vertically or horizontally.
         * </p>
         */
        public void Resize()
        {
            Resize(1.0);
        }

        /**
         * Reset the image to the original size.
         * <p>
         * Please note, that this method works correctly only for workbooks
         * with the default font size (Calibri 11pt for .xlsx).
         * If the default font is Changed the resized image can be streched vertically or horizontally.
         * </p>
         *
         * @param scale the amount by which image dimensions are multiplied relative to the original size.
         * <code>resize(1.0)</code> Sets the original size, <code>resize(0.5)</code> resize to 50% of the original,
         * <code>resize(2.0)</code> resizes to 200% of the original.
         */
        public void Resize(double scale)
        {
            XSSFClientAnchor anchor = (XSSFClientAnchor)GetAnchor();

            XSSFClientAnchor pref = GetPreferredSize(scale);

            int row2 = anchor.Row1 + (pref.Row2 - pref.Row1);
            int col2 = anchor.Col1 + (pref.Col2 - pref.Col1);

            anchor.Col2=(col2);
            anchor.Dx1=(0);
            anchor.Dx2=(pref.Dx2);

            anchor.Row2=(row2);
            anchor.Dy1=(0);
            anchor.Dy2=(pref.Dy2);
        }

        /**
         * Calculate the preferred size for this picture.
         *
         * @return XSSFClientAnchor with the preferred size for this image
         */
        public XSSFClientAnchor GetPreferredSize()
        {
            return GetPreferredSize(1);
        }

        /**
         * Calculate the preferred size for this picture.
         *
         * @param scale the amount by which image dimensions are multiplied relative to the original size.
         * @return XSSFClientAnchor with the preferred size for this image
         */
        public XSSFClientAnchor GetPreferredSize(double scale)
        {
            XSSFClientAnchor anchor = (XSSFClientAnchor)GetAnchor();

            XSSFPictureData data = GetPictureData();
            Size size = GetImageDimension(data.GetPackagePart(), data.GetPictureType());
            double scaledWidth = size.Width * scale;
            double scaledHeight = size.Height * scale;

            float w = 0;
            int col2 = anchor.Col1;
            int dx2 = 0;

            for (; ; )
            {
                w += GetColumnWidthInPixels(col2);
                if (w > scaledWidth) break;
                col2++;
            }

            if (w > scaledWidth)
            {
                double cw = GetColumnWidthInPixels(col2);
                double delta = w - scaledWidth;
                dx2 = (int)(EMU_PER_PIXEL * (cw - delta));
            }
            anchor.Col2 = (col2);
            anchor.Dx2 = (dx2);

            double h = 0;
            int row2 = anchor.Row1;
            int dy2 = 0;

            for (; ; )
            {
                h += GetRowHeightInPixels(row2);
                if (h > scaledHeight) break;
                row2++;
            }

            if (h > scaledHeight)
            {
                double ch = GetRowHeightInPixels(row2);
                double delta = h - scaledHeight;
                dy2 = (int)(EMU_PER_PIXEL * (ch - delta));
            }
            anchor.Row2 = (row2);
            anchor.Dy2 = (dy2);

            CT_PositiveSize2D size2d = ctPicture.spPr.xfrm.ext;
            size2d.cx = ((long)(scaledWidth * EMU_PER_PIXEL));
            size2d.cy = ((long)(scaledHeight * EMU_PER_PIXEL));

            return anchor;
        }

        private float GetColumnWidthInPixels(int columnIndex)
        {
            XSSFSheet sheet = (XSSFSheet)GetDrawing().GetParent();

            CT_Col col = sheet.GetColumnHelper().GetColumn(columnIndex, false);
            double numChars = col == null || !col.IsSetWidth() ? DEFAULT_COLUMN_WIDTH : col.width;

            return (float)numChars * XSSFWorkbook.DEFAULT_CHARACTER_WIDTH;
        }

        private float GetRowHeightInPixels(int rowIndex)
        {
            XSSFSheet sheet = (XSSFSheet)GetDrawing().GetParent();

            XSSFRow row = sheet.GetRow(rowIndex);
            float height = row != null ? row.GetHeightInPoints() : sheet.GetDefaultRowHeightInPoints();
            return height * PIXEL_DPI / POINT_DPI;
        }

        /**
         * Return the dimension of this image
         *
         * @param part the namespace part holding raw picture data
         * @param type type of the picture: {@link Workbook#PICTURE_TYPE_JPEG},
         * {@link Workbook#PICTURE_TYPE_PNG} or {@link Workbook#PICTURE_TYPE_DIB}
         *
         * @return image dimension in pixels
         */
        protected static Size GetImageDimension(PackagePart part, int type)
        {
            try
            {
                return Image.FromStream(part.GetInputStream()).Size;
            }
            catch (IOException e)
            {
                //return a "singulariry" if ImageIO failed to read the image
                logger.Log(POILogger.WARN, e);
                return new Size();
            }
        }

        /**
         * Return picture data for this shape
         *
         * @return picture data for this shape
         */
        public XSSFPictureData GetPictureData()
        {
            String blipId = ctPicture.blipFill.blip.embed;
            foreach (POIXMLDocumentPart part in GetDrawing().GetRelations())
            {
                if (part.GetPackageRelationship().Id.Equals(blipId))
                {
                    return (XSSFPictureData)part;
                }
            }
            logger.Log(POILogger.WARN, "Picture data was not found for blipId=" + blipId);
            return null;
        }

        protected CT_ShapeProperties GetShapeProperties()
        {
            return ctPicture.spPr;
        }

    }
}

