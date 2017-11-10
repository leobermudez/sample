using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace CodingTest
{
    [SerializableAttribute]
    [ComVisibleAttribute(true)]
    public class Node
    {
        static List<Node> allNodes = new List<Node>();
        public int Id { get; set; }
        public Point Locate { get; set; }
        public int NodeValue { get; set; }
        public int ParentID { get; set; }
        public int Level { get; set; }
        public Node(int id, Point loc, int nodeValue, int parentId, int level)
        {
            Id = id;
            Locate = loc;
            NodeValue = nodeValue;
            ParentID = parentId;
            Level = level;
        }
    }
    public class Point
    {
        public int x { get; set; }
        public int y { get; set; }
        public Point(int p1, int p2)
        {
            x = p1;
            y = p2;
        }
       
    }    
    class Program
    {
       
        static List<Node> allNodes = new List<Node>();
        static int[][] points;
        static int runningValueID = 1;
        static int runningLevel = 0;
        static void getMap()
        {
            string map;
            var fileStream = new FileStream(@"c:\developer\map.txt", FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                map = streamReader.ReadToEnd();
            }
            Console.WriteLine(map);
        }
       
        static void Main(string[] args)
        {
            getMap();
            int startpt;
            Console.Write("Enter starting point: ");
            startpt = Int32.Parse(Console.ReadLine());
            try
             {
                
                points = ReadFile();
                for (int i = 0; i < points.Length; i++)
                {
                    for (int j = 0; j < points[i].Length; j++)
                    {
                        points[i][j] = startpt;
                        AddBaseNodeLevel(new Point(i, j), points[i][j]);
                        
                    }
                }
                
                while (AddSubNode() > 0)
                {

                    var longestPath = allNodes.OrderByDescending(x => x.Level).FirstOrDefault().Level;
                    var steepestPathNodes = new List<int[]>();
                    allNodes.Where(y => y.Level == longestPath).OrderByDescending(x => x.Id).ToList().ForEach(y => steepestPathNodes.Add(GetTreeNodeValues(y)));
                    var steepestPath = steepestPathNodes.OrderByDescending(x => x.Max() - x.Min()).FirstOrDefault();
                    Console.WriteLine("Longest path length is : {0} and the steepest drop is : {1} ", longestPath + 1, steepestPath.Max() - steepestPath.Min());
                    Console.WriteLine("The path is :");
                    steepestPath.ToList().ForEach(x => Console.WriteLine(x));
                   // Console.WriteLine(steepestPath); 
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something's  wrong! {0}", ex.Message);
                Console.ReadKey();
            }
        }
        
        static int[][] ReadFile()
        {

            var filePath = @"c:\developer\map.txt";

            return File.ReadLines(filePath).Skip(1).Select(x => x.Split(' ').Select(y => int.Parse(y)).ToArray()).ToArray();
        } 
        static int[] GetTreeNodeValues(Node node)
        {
            var nodePath = new List<int>() { node.NodeValue };
            Node parentNode = null;
            do
            {
                parentNode = allNodes.Where(x => x.Id == node.ParentID).SingleOrDefault();
                if (parentNode != null)
                    nodePath.Add(parentNode.NodeValue);
                node = parentNode;
            } while (parentNode != null);
            return nodePath.ToArray();
        }

        static void AddBaseNodeLevel(Point node, int nodeValue)
        {
            allNodes.Add(new Node(runningValueID++, node, nodeValue, 0, 0));
        }

        static int AddSubNode()
        {
            int count = 0;
            allNodes.Where(x => x.Level == runningLevel).ToArray().ToList().ForEach(x =>
            {
                count += AddNextNode(pathdirection.north, x) + AddNextNode(pathdirection.south, x) + AddNextNode(pathdirection.east, x) + AddNextNode(pathdirection.west, x);
            });
            runningLevel++;
            return count;
        }

        static int AddNextNode(pathdirection dir, Node node)
        {
            Point nextNode = FindNext(dir, node.Locate);
            if (nextNode != null && points[nextNode.x][nextNode.y] < node.NodeValue)
            {
                allNodes.Add(new Node(runningValueID++, nextNode, points[nextNode.x][nextNode.y], node.Id, runningLevel + 1));
                return 1;
            }
            return 0;
        }

        static Point FindNext(pathdirection dir, Point node)
        {
            Point nextLoc = null;
            switch (dir)
            {
                case pathdirection.north:
                    nextLoc = node.x - 1 >= 0 ? new Point(node.x - 1, node.y) : null;
                    break;
                case pathdirection.south:
                    nextLoc = node.x + 1 < points.GetLength(0) ? new Point(node.x + 1, node.y) : null;
                    break;
                case pathdirection.east:
                    nextLoc = node.y + 1 < points.GetLength(0) ? new Point(node.x, node.y + 1) : null;
                    break;
                case pathdirection.west:
                    nextLoc = node.y - 1 >= 0 ? new Point(node.x, node.y - 1) : null;
                    break;
                default:
                    break;
            }
            return nextLoc;

        }


    }



    enum pathdirection { north, south, east, west }
    

}

