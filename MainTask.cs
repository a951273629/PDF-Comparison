using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace iTextSharpReadPDF
{
    class MainTask
    {
        private static readonly object lockObj = new object(); // 创建一个对象用作互斥锁
        private static SaveResult saveResult;

        public static void startTask()
        {
            Task task = Task.Run(() =>
            {
                mainTask();
            });
            //Task.WaitAll();
            task.Wait();
            Console.WriteLine("Task is finished");
            saveResult.saveResultAsExcel();
            Console.ReadKey();
        }
        public static void mainTask()
        {
            // 获取当前系统上可用的处理器数量
            //int processorCount = Environment.ProcessorCount;

            //// 设置线程池的最大工作线程数为处理器数量
            ThreadPool.SetMaxThreads(48, 48);
            //Console.WriteLine(processorCount);
            CaculateContain caculateContain = new CaculateContain();

            string excelFilePath = @"D:\VisualStudioRepos\ReadPdf\EGA workitems1.xlsx";
            string pefFilePath = @"D:\VisualStudioRepos\iTextSharpReadPDF\draft.PDF"; ; // 替换为你的 PDF 文件路径
            string saveFile = @"D:\VisualStudioRepos\iTextSharpReadPDF\fileResult.xlsx";
            PdfEnumberToString pdfEnumberToString = new PdfEnumberToString(pefFilePath);
            saveResult = new SaveResult(saveFile);
            var e = new ExcelRead(excelFilePath);
            Dictionary<string, RowInfo> dictionary = e.readExcel();
            List<pdfText> pdfTexts = pdfEnumberToString.readPdf();
            //dictionary.Keys.Count
            CountdownEvent countdownEvent = new CountdownEvent(dictionary.Keys.Count);
            WaitCallback task = (object obj) =>
            {
                string item = (string)obj;
                string resultMatchConten = "";
                string orginContent = "";
                double matchContentPercent = 0.0;
                int matchPage = 0;


                bool titleExist = false;
                int maxContent = 0;
                //for (int i = 0; i < pdfTexts.Count; i++)
                foreach (var pdfText in pdfTexts)
                {


                    if (!titleExist && caculateContain.isExistSameString(item, pdfText.text))
                    {
                        titleExist = true;
                        //Console.WriteLine($"{item}\n-------\n{pdfText.text}\n-------\nPage:{pdfText.page}");
                        //break;
                    }
                    MaxInfo info = caculateContain.commonSameBest(dictionary[item].value, pdfText.text);
                    if (maxContent <= info.max)
                    {
                        maxContent = info.max;
                        orginContent = dictionary[item].value;
                        resultMatchConten = caculateContain.commonSameString(pdfText.text, info);
                        matchContentPercent = Math.Round((double)maxContent * 100 / (double)orginContent.Length, 2);
                        matchPage = pdfText.page;
                    }

                }
                Result result = new Result(orginContent: orginContent, matchContent: resultMatchConten, matchContentPercent: matchContentPercent
                    , matchPage: matchPage, matchRow: dictionary[item].row, matchTitle: dictionary[item].value, titleExist: titleExist);
                //lock (lockObj)
                //{
                Console.WriteLine(result);
                saveResult.addResult(result);
                //}
                //完成任务减一
                countdownEvent.Signal();

            };
            int index = 0;

            //Console.WriteLine(pdfTexts.Count);
            foreach (var item in dictionary.Keys)
            {
                index++;
                ThreadPool.QueueUserWorkItem(task, item);
                //if (index == 5) break;
            }
            //等待任务完成
            countdownEvent.Wait();
            //Dictionary<string, RowInfo>.KeyCollection.Enumerator enumerator = dictionary.Keys.GetEnumerator();
            //enumerator.MoveNext

            //saveResult.saveResultAsExcel();


        }
    }
}
