using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SurfaceApp1
{
    public class GraphReader
    {
        // TODO - return a graph structure
        // @see http://arxiv.org/ftp/arxiv/papers/0908/0908.3089.pdf
        public static void readGraphFile(String file)
        {
            using (TextReader reader = File.OpenText(file))
            {
                // TODO - see below for examples of reading.
                //int x = int.Parse(reader.ReadLine());
                //double y = double.Parse(reader.ReadLine());
                //string z = reader.ReadLine();
            }
        }
    }
}
