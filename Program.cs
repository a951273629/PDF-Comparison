using System;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Collections.Generic;
namespace iTextSharpReadPDF
{
    class Program
    {
        public static int _numberOfCharsToKeep { get; private set; }

        //static object consoleLock = new object();
        static void Main(string[] args)
        {
            //CaculateContain caculateContain = new CaculateContain();
            //string str1 = "xxxbcdefxxx";
            //string str2 = "65758abcdefghijkl";
            //MaxInfo maxInfo = caculateContain.commonSameBest(str1, str2);
            //Console.WriteLine(maxInfo);
            //Console.WriteLine(caculateContain.commonSameString(str1, str2));
            MainTask.startTask();
            //SaveResult saveResult = new SaveResult("");
            //saveResult.saveResultAsExcel();

            Console.ReadKey();

        }
        static void pdfRead()
        {
            string filePath = @"D:\VisualStudioRepos\iTextSharpReadPDF\draft.PDF"; ; // 替换为你的 PDF 文件路径
            StringBuilder stringBuilder = new StringBuilder();
            //StreamUtil.add
            //System.Reflection.Assembly.Load("123");
            // reader ==>                 http://itextsupport.com/apidocs/itext5/5.5.9/com/itextpdf/text/pdf/PdfReader.html#pdfVersion
            PdfReader reader = new PdfReader(filePath);
            var strategy = new LocationTextExtractionStrategy();
            //strategy==> http://itextsupport.com/apidocs/itext5/5.5.9/com/itextpdf/text/pdf/parser/TextExtractionStrategy.html
            //    for (int i = 1; i <= reader.NumberOfPages; i++)
            //   {
            int pageNumber = reader.NumberOfPages;
            Console.WriteLine(pageNumber);

            for (int i = 1; i <= pageNumber; i++)
            {
                string text = PdfTextExtractor.GetTextFromPage(reader, i/*i*/, strategy);

                bool titleExist = false;

                if (i == 34)
                {
                    stringBuilder.Append($"{text} \n\n page:{i}\n\n");
                    break;
                }
            }

            //  PdfTextExtractor.GetTextFromPage(reader, 6, S) ==>>    http://itextsupport.com/apidocs/itext5/5.5.9/com/itextpdf/text/pdf/parser/PdfTextExtractor.html
            Console.WriteLine(stringBuilder.ToString());
            Console.ReadKey();
        }
        //static void pdfiText()
        //{
        //    StringBuilder text = new StringBuilder();
        //    try
        //    {
        //        using (FileStream pdfFile = new FileStream("", FileMode.Open, FileAccess.Read))
        //        {

        //            MemoryStream outputStream = new MemoryStream();

        //            using (PdfReader reader = new PdfReader(pdfFile))
        //            {
        //                PdfDocument pdfDocument = new PdfDocument(reader);
        //                // 获取 PDF 页面数量
        //                int numPages = pdfDocument.GetNumberOfPages();

        //                // 遍历每一页并提取文本
        //                for (int i = 1; i <= numPages; i++)
        //                {
        //                    // 获取页面内容
        //                    var strategy = new LocationTextExtractionStrategy();
        //                    //MyTextExtractor myTextExtractor = new MyTextExtractor();
        //                    //strategy.GetResultantText();
        //                    //pdfDocument.GetPage(i)
        //                    //re.

        //                    //reader.ReadStreamBytes();
        //                    //string currentPageText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i), strategy);
        //                    string currentPageText = ExtractTextFromPDFBytes(pdfDocument.GetPage(i).GetContentBytes());

        //                    // 将当前页面的文本追加到字符串构建器
        //                    text.Append(currentPageText);

        //                }
        //                Console.WriteLine(text.ToString());
        //                Console.ReadKey();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("发生异常：" + ex.Message);
        //    }

        //}
        private static string ExtractTextFromPDFBytes(byte[] input)
        {
            if (input == null || input.Length == 0) return "";
            try
            {
                string resultString = "";
                // Flag showing if we are we currently inside a text object
                bool inTextObject = false;
                // Flag showing if the next character is literal  e.g. '\\' to get a '\' character or '\(' to get '('
                bool nextLiteral = false;
                // () Bracket nesting level. Text appears inside ()
                int bracketDepth = 0;
                // Keep previous chars to get extract numbers etc.:
                char[] previousCharacters = new char[_numberOfCharsToKeep];
                for (int j = 0; j < _numberOfCharsToKeep; j++) previousCharacters[j] = ' ';
                for (int i = 0; i < input.Length; i++)
                {
                    char c = (char)input[i];
                    if (inTextObject)
                    {
                        // Position the text
                        if (bracketDepth == 0)
                        {
                            if (CheckToken(new string[] { "TD", "Td" }, previousCharacters))
                            {
                                resultString += "\n\r";
                            }
                            else
                            {
                                if (CheckToken(new string[] { "'", "T*", "\"" }, previousCharacters))
                                {
                                    resultString += "\n";
                                }
                                else
                                {
                                    if (CheckToken(new string[] { "Tj" }, previousCharacters))
                                    {
                                        resultString += " ";
                                    }
                                }
                            }
                        }
                        // End of a text object, also go to a new line.
                        if (bracketDepth == 0 && CheckToken(new string[] { "ET" }, previousCharacters))
                        {
                            inTextObject = false;
                            resultString += " ";
                        }
                        else
                        {
                            // Start outputting text
                            if ((c == '(') && (bracketDepth == 0) && (!nextLiteral))
                            {
                                bracketDepth = 1;
                            }
                            else
                            {
                                // Stop outputting text
                                if ((c == ')') && (bracketDepth == 1) && (!nextLiteral))
                                {
                                    bracketDepth = 0;
                                }
                                else
                                {
                                    // Just a normal text character:
                                    if (bracketDepth == 1)
                                    {
                                        // Only print out next character no matter what. 
                                        // Do not interpret.
                                        if (c == '\\' && !nextLiteral)
                                        {
                                            nextLiteral = true;
                                        }
                                        else
                                        {
                                            if (((c >= ' ') && (c <= '~')) || ((c >= 128) && (c < 255)))
                                            {
                                                resultString += c.ToString();
                                            }
                                            nextLiteral = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Store the recent characters for when we have to go back for a checking
                    for (int j = 0; j < _numberOfCharsToKeep - 1; j++)
                    {
                        previousCharacters[j] = previousCharacters[j + 1];
                    }
                    previousCharacters[_numberOfCharsToKeep - 1] = c;

                    // Start of a text object
                    if (!inTextObject && CheckToken(new string[] { "BT" }, previousCharacters))
                    {
                        inTextObject = true;
                    }
                }
                return resultString;
            }
            catch
            {
                return "";
            }
        }

        private static bool CheckToken(string[] tokens, char[] recent)
        {
            foreach (string token in tokens)
            {
                if ((recent[_numberOfCharsToKeep - 3] == token[0]) &&
                    (recent[_numberOfCharsToKeep - 2] == token[1]) &&
                    ((recent[_numberOfCharsToKeep - 1] == ' ') ||
                    (recent[_numberOfCharsToKeep - 1] == 0x0d) ||
                    (recent[_numberOfCharsToKeep - 1] == 0x0a)) &&
                    ((recent[_numberOfCharsToKeep - 4] == ' ') ||
                    (recent[_numberOfCharsToKeep - 4] == 0x0d) ||
                    (recent[_numberOfCharsToKeep - 4] == 0x0a))
                    )
                {
                    return true;
                }
            }
            return false;
        }
        void testOcr()
        {
            //int[] arrange = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            //IronOcr.License.LicenseKey = "IRONSUITE.A951273629.GMAIL.COM.22534-A30BBFE538-BUMER-J5ZQ3JOIB3TZ-2BORDBE56PBR-4Q2IJERKYHGP-7TNSARNZNG7O-SSD7THMI7PEI-37WIS3HA4VOO-6VRH52-TQ22ZNKB6LCLUA-DEPLOYMENT.TRIAL-TJ3INW.TRIAL.EXPIRES.03.FEB.2024";

            //bool result = IronOcr.License.IsValidLicense(IronOcr.License.LicenseKey);
            //Console.WriteLine(result);

            ////ocrInput.AddImage("image.png");
            ////ocrInput.AddPdf(filePath);
            //for (int i = 0; i < 5; i++)
            //{

            //    ThreadPool.QueueUserWorkItem(ReadTask, arrange);
            //    Thread.Sleep(3000);
            //    //Task.Delay(5000);
            //    for (int j = 0; j < arrange.Length; j++)
            //    {
            //        arrange[j] += 20;
            //    }

            //}
            //Console.WriteLine("task already start");
            //Console.ReadKey();
            //Console.ReadLine();
        }
        static void ReadTask(object progm)
        {
            //int[] arr = (int[])progm;
            //var ocr = new IronTesseract();
            //// Fast Dictionary
            ////ocr.Language = OcrLanguage.EnglishFast;
            ////ocr.MultiThreaded = true;
            //// 指定要读取的 PDF 文件路径
            //string filePath = @"D:\VisualStudioRepos\iTextSharpReadPDF\draft.PDF";
            //try
            //{

            //    using (var ocrInput = new OcrInput())
            //    {

            //        lock (consoleLock)
            //        {
            //            ocrInput.AddPdfPages(filePath, arr);
            //            var ocrResult = ocr.Read(ocrInput);
            //            Console.WriteLine(ocrResult.Text);
            //        }

            //        //ocrInput.RemovePages(arr);
            //    }
            //}
            //catch (Exception)
            //{
            //    Console.WriteLine("error");
            //    throw;
            //}
        }
        //static string GetStringFromPdfDictionary(PdfDictionary pdfDict)
        //{
        //    string dictAsString = "";

        //    foreach (PdfName key in pdfDict.KeySet())
        //    {
        //        PdfObject obj = pdfDict.Get(key);

        //        string keyString = GetStringFromPdfObject(obj);
        //        //string valueString = GetValueAsString(obj);

        //        dictAsString += keyString;
        //    }

        //    return dictAsString;
        //}
        //static void showOutLine(PdfOutline pdfOutline)
        //{
        //    //Console.WriteLine(11 + "/n");
        //    foreach (PdfOutline item in pdfOutline.GetAllChildren())
        //    {

        //        Console.WriteLine(item.GetTitle());
        //        if (item != null)
        //        {
        //            showOutLine(item);
        //        }
        //    }

        //}
        //static string GetStringFromPdfObject(PdfObject pdfObject)
        //{
        //    if (pdfObject != null)
        //    {
        //        switch (pdfObject.GetObjectType())
        //        {
        //            case PdfObject.ARRAY:
        //                return "this is Array";
        //            //GetStringFromPdfArray((PdfArray)pdfObject);
        //            case PdfObject.BOOLEAN:
        //                return ((PdfBoolean)pdfObject).GetValue().ToString();
        //            case PdfObject.DICTIONARY:
        //                return GetStringFromPdfDictionary((PdfDictionary)pdfObject);
        //            case PdfObject.NAME:
        //                return ((PdfName)pdfObject).GetValue();
        //            case PdfObject.NULL:
        //                return "null";
        //            case PdfObject.NUMBER:
        //                return ((PdfNumber)pdfObject).GetValue().ToString();
        //            case PdfObject.STREAM:
        //                // 如果是流对象，可能包含大量数据，这里仅作示例直接输出了类型
        //                return "Stream Object";
        //            case PdfObject.STRING:
        //                return ((PdfString)pdfObject).GetValue();
        //            case PdfObject.INDIRECT_REFERENCE:
        //                return pdfObject.ToString();
        //            default:
        //                return "Unknown Type";
        //        }
        //    }
        //    else
        //    {
        //        return "Object not found or null";
        //    }
        //}

    }



}
