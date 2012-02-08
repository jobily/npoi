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
using NPOI.Util;
using System.Collections.Generic;
using System;
using NPOI.OpenXml4Net.OPC;
using NPOI.XSSF.Model;
using System.IO;
namespace NPOI.XSSF.UserModel
{

    /**
     *
     */
    public class XSSFRelation : POIXMLRelation
    {

        private static POILogger log = POILogFactory.GetLogger(typeof(XSSFRelation));

        /**
         * A map to lookup POIXMLRelation by its relation type
         */
        protected static Dictionary<String, XSSFRelation> _table = new Dictionary<String, XSSFRelation>();


        public static XSSFRelation WORKBOOK = new XSSFRelation(
                "application/vnd.Openxmlformats-officedocument.spreadsheetml.sheet.main+xml",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/workbook",
                "/xl/workbook.xml",
                null
        );
        public static XSSFRelation MACROS_WORKBOOK = new XSSFRelation(
                "application/vnd.ms-excel.sheet.macroEnabled.main+xml",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/officeDocument",
                "/xl/workbook.xml",
                null
        );
        public static XSSFRelation TEMPLATE_WORKBOOK = new XSSFRelation(
                  "application/vnd.Openxmlformats-officedocument.spreadsheetml.template.main+xml",
                  "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/officeDocument",
                  "/xl/workbook.xml",
                  null
        );
        public static XSSFRelation MACRO_TEMPLATE_WORKBOOK = new XSSFRelation(
                  "application/vnd.ms-excel.template.macroEnabled.main+xml",
                  "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/officeDocument",
                  "/xl/workbook.xml",
                  null
        );
        public static XSSFRelation MACRO_ADDIN_WORKBOOK = new XSSFRelation(
                  "application/vnd.ms-excel.Addin.macroEnabled.main+xml",
                  "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/officeDocument",
                  "/xl/workbook.xml",
                  null
        );
        public static XSSFRelation WORKSHEET = new XSSFRelation(
                "application/vnd.Openxmlformats-officedocument.spreadsheetml.worksheet+xml",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/worksheet",
                "/xl/worksheets/sheet#.xml",
                typeof(XSSFSheet)
        );
        public static XSSFRelation CHARTSHEET = new XSSFRelation(
                "application/vnd.Openxmlformats-officedocument.spreadsheetml.chartsheet+xml",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/chartsheet",
                "/xl/chartsheets/sheet#.xml",
                typeof(XSSFChartSheet)
        );
        public static XSSFRelation SHARED_STRINGS = new XSSFRelation(
                "application/vnd.Openxmlformats-officedocument.spreadsheetml.sharedStrings+xml",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/sharedStrings",
                "/xl/sharedStrings.xml",
                typeof(SharedStringsTable)
        );
        public static XSSFRelation STYLES = new XSSFRelation(
                "application/vnd.Openxmlformats-officedocument.spreadsheetml.styles+xml",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/styles",
                "/xl/styles.xml",
                typeof(StylesTable)
        );
        public static XSSFRelation DRAWINGS = new XSSFRelation(
                "application/vnd.Openxmlformats-officedocument.Drawing+xml",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/drawing",
                "/xl/drawings/drawing#.xml",
                typeof(XSSFDrawing)
        );
        public static XSSFRelation VML_DRAWINGS = new XSSFRelation(
                "application/vnd.Openxmlformats-officedocument.vmlDrawing",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/vmlDrawing",
                "/xl/drawings/vmlDrawing#.vml",
                typeof(XSSFVMLDrawing)
        );
        public static XSSFRelation CHART = new XSSFRelation(
              "application/vnd.Openxmlformats-officedocument.Drawingml.chart+xml",
              "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/chart",
              "/xl/charts/chart#.xml",
              typeof(XSSFChart)
        );

        public static XSSFRelation CUSTOM_XML_MAPPINGS = new XSSFRelation(
                "application/xml",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/xmlMaps",
                "/xl/xmlMaps.xml",
                typeof(MapInfo)
        );

        public static XSSFRelation SINGLE_XML_CELLS = new XSSFRelation(
                "application/vnd.Openxmlformats-officedocument.spreadsheetml.tableSingleCells+xml",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/tableSingleCells",
                "/xl/tables/tableSingleCells#.xml",
                typeof(SingleXmlCells)
        );

        public static XSSFRelation TABLE = new XSSFRelation(
                "application/vnd.Openxmlformats-officedocument.spreadsheetml.table+xml",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/table",
                "/xl/tables/table#.xml",
                typeof(XSSFTable)
        );

        public static XSSFRelation IMAGES = new XSSFRelation(
                null,
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/image",
                null,
                typeof(XSSFPictureData)
        );
        public static XSSFRelation IMAGE_EMF = new XSSFRelation(
                "image/x-emf",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/image",
                "/xl/media/image#.emf",
                typeof(XSSFPictureData)
        );
        public static XSSFRelation IMAGE_WMF = new XSSFRelation(
                "image/x-wmf",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/image",
                "/xl/media/image#.wmf",
                typeof(XSSFPictureData)
        );
        public static XSSFRelation IMAGE_PICT = new XSSFRelation(
                "image/pict",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/image",
                "/xl/media/image#.pict",
                typeof(XSSFPictureData)
        );
        public static XSSFRelation IMAGE_JPEG = new XSSFRelation(
                "image/jpeg",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/image",
                "/xl/media/image#.jpeg",
                typeof(XSSFPictureData)
        );
        public static XSSFRelation IMAGE_PNG = new XSSFRelation(
                "image/png",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/image",
                "/xl/media/image#.png",
                typeof(XSSFPictureData)
        );
        public static XSSFRelation IMAGE_DIB = new XSSFRelation(
                "image/dib",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/image",
                "/xl/media/image#.dib",
                typeof(XSSFPictureData)
        );

        public static XSSFRelation SHEET_COMMENTS = new XSSFRelation(
                  "application/vnd.Openxmlformats-officedocument.spreadsheetml.comments+xml",
                  "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/comments",
                  "/xl/comments#.xml",
                  typeof(CommentsTable)
          );
        public static XSSFRelation SHEET_HYPERLINKS = new XSSFRelation(
                null,
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/hyperlink",
                null,
                null
        );
        public static XSSFRelation OLEEMBEDDINGS = new XSSFRelation(
                null,
                POIXMLDocument.OLE_OBJECT_REL_TYPE,
                null,
                null
        );
        public static XSSFRelation PACKEMBEDDINGS = new XSSFRelation(
                null,
                POIXMLDocument.PACK_OBJECT_REL_TYPE,
                null,
                null
        );

        public static XSSFRelation VBA_MACROS = new XSSFRelation(
                "application/vnd.ms-office.vbaProject",
                "http://schemas.microsoft.com/office/2006/relationships/vbaProject",
                "/xl/vbaProject.bin",
                null
        );
        public static XSSFRelation ACTIVEX_CONTROLS = new XSSFRelation(
                "application/vnd.ms-office.activeX+xml",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/control",
                "/xl/activeX/activeX#.xml",
                null
        );
        public static XSSFRelation ACTIVEX_BINS = new XSSFRelation(
                "application/vnd.ms-office.activeX",
                "http://schemas.microsoft.com/office/2006/relationships/activeXControlBinary",
                "/xl/activeX/activeX#.bin",
                null
        );
        public static XSSFRelation THEME = new XSSFRelation(
                "application/vnd.Openxmlformats-officedocument.theme+xml",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/theme",
                "/xl/theme/theme#.xml",
                typeof(ThemesTable)
        );
        public static XSSFRelation CALC_CHAIN = new XSSFRelation(
                "application/vnd.Openxmlformats-officedocument.spreadsheetml.calcChain+xml",
                "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/calcChain",
                "/xl/calcChain.xml",
                typeof(CalculationChain)
        );
        public static XSSFRelation PRINTER_SETTINGS = new XSSFRelation(
              "application/vnd.Openxmlformats-officedocument.spreadsheetml.printerSettings",
              "http://schemas.Openxmlformats.org/officeDocument/2006/relationships/printerSettings",
              "/xl/printerSettings/printerSettings#.bin",
              null
       );


        private XSSFRelation(String type, String rel, String defaultName, Type cls) :
            base(type, rel, defaultName, cls)
        {


            if (cls != null && !_table.ContainsKey(rel)) 
                _table[rel]= this;
        }

        /**
         *  Fetches the InputStream to read the contents, based
         *  of the specified core part, for which we are defined
         *  as a suitable relationship
         */
        public Stream GetContents(PackagePart corePart)
        {
            PackageRelationshipCollection prc =
                corePart.GetRelationshipsByType(_relation);
            IEnumerator<PackageRelationship> it = prc.GetEnumerator();
            if (it.MoveNext())
            {
                PackageRelationship rel = it.Current;
                PackagePartName relName = PackagingUriHelper.CreatePartName(rel.TargetUri);
                PackagePart part = corePart.Package.GetPart(relName);
                return part.GetInputStream();
            }
            log.Log(POILogger.WARN, "No part " + _defaultName + " found");
            return null;
        }


        /**
         * Get POIXMLRelation by relation type
         *
         * @param rel relation type, for example,
         *    <code>http://schemas.Openxmlformats.org/officeDocument/2006/relationships/image</code>
         * @return registered POIXMLRelation or null if not found
         */
        public static XSSFRelation GetInstance(String rel)
        {
            return _table[rel];
        }
    }
}


