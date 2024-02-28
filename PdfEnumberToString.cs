using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace iTextSharpReadPDF
{
    class PdfEnumberToString
    {
        private int currentIndex = 1;
        private int count = 0;
        private string filePath;
        private PdfReader reader = null;
        public PdfEnumberToString(string filePath)
        {
            this.filePath = filePath;
            this.reader = new PdfReader(this.filePath);
            this.count = this.reader.NumberOfPages;
        }

        public pdfText Current
        {
            get;
            set;
        }


        void pdfRead()
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



        public bool MoveNext()
        {
            if (this.currentIndex <= this.count)
            {
                string text = PdfTextExtractor.GetTextFromPage(this.reader, this.currentIndex, new LocationTextExtractionStrategy());
                this.Current = new pdfText(text, currentIndex);
                this.currentIndex++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            currentIndex = 1;
        }
        public List<pdfText> readPdf()
        {
            List<pdfText> pdfTexts = new List<pdfText>();
            while (MoveNext())
            {

                pdfTexts.Add(Current);
            }
            return pdfTexts;
        }
    }
    class pdfText
    {
        public string text { get; set; }
        public int page { get; set; }

        public pdfText()
        {
        }

        public pdfText(string text, int page)
        {
            this.text = text;
            this.page = page;
        }
    }

}
