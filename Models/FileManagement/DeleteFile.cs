using System;
using System.IO;
using System.Threading;

namespace CoreBot.Models.FileManagement
{
    public class DeleteFile
    {
        public void Delete(string name)
        {
            try {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/temp/", name);

                System.Threading.Thread t = new System.Threading.Thread(() => {
                    System.Threading.Thread.Sleep(100000);
                    System.IO.File.Delete(fullPath);
                });

                t.Start();
            
            } catch(IOException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
