using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

public class Graph
{
    public int Vertices { get; }
    public List<(int, int, int)> Edges { get; }

    public Graph(int vertices)
    {
        Vertices = vertices;
        Edges = new List<(int, int, int)>();
    }

    public void AddEdge(int u, int v, int weight)
    {
        Edges.Add((u, v, weight));
    }

    public List<(int, int, int)> GetEdges() => Edges;
}

public class UnionFind
{
    private int[] parent;
    private int[] rank;

    public UnionFind(int n)
    {
        parent = new int[n];
        rank = new int[n];

        for (int i = 0; i < n; i++)
        {
            parent[i] = i;
            rank[i] = 0;
        }
    }

    public int Find(int x)
    {
        if (parent[x] != x)
        {
            parent[x] = Find(parent[x]);
        }
        return parent[x];
    }

    public void Union(int x, int y)
    {
        int rootX = Find(x);
        int rootY = Find(y);

        if (rootX != rootY)
        {
            if (rank[rootX] > rank[rootY])
            {
                parent[rootY] = rootX;
            }
            else if (rank[rootX] < rank[rootY])
            {
                parent[rootX] = rootY;
            }
            else
            {
                parent[rootY] = rootX;
                rank[rootX]++;
            }
        }
    }
}

public class KruskalAlgorithm
{
    public static (int totalWeight, List<(int, int, int)> mstEdges) Run(Graph graph)
    {
        int totalWeight = 0;
        List<(int, int, int)> mstEdges = new List<(int, int, int)>();

        
        var sortedEdges = graph.Edges.OrderBy(e => e.Item3).ToList();

        
        UnionFind uf = new UnionFind(graph.Vertices + 1); 

        foreach (var (u, v, weight) in sortedEdges)
        {
            if (uf.Find(u) != uf.Find(v))  
            {
                uf.Union(u, v);  
                mstEdges.Add((u, v, weight)); 
                totalWeight += weight; 
            }
        }

        return (totalWeight, mstEdges);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Por favor, forneça o caminho do arquivo como argumento.");
            return;
        }

        string filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Arquivo não encontrado.");
            return;
        }

        var graph = ReadGraphFromFile(filePath);

        Stopwatch stopwatch = Stopwatch.StartNew();

        var (mstWeight, mstEdges) = KruskalAlgorithm.Run(graph);

        stopwatch.Stop();

        Console.WriteLine("\n\n\n");
        Console.WriteLine("=================   Resumo   =====================");
        Console.WriteLine($"Tempo de execução: {stopwatch.ElapsedMilliseconds} ms");

        Console.WriteLine($"Número de vértices: {graph.Vertices}");
        Console.WriteLine($"Número de arestas: {graph.Edges.Count}");

        Console.WriteLine($"Peso da Árvore Geradora Mínima (MST): {mstWeight}");

        Console.WriteLine("\nArestas da Árvore Geradora Mínima:");
        foreach (var edge in mstEdges)
        {
            Console.WriteLine($"Vértice {edge.Item1} - Vértice {edge.Item2} com peso {edge.Item3}");
        }
    }

    public static Graph ReadGraphFromFile(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        int vertices = 0, edges = 0;

        var graph = new Graph(0);
        foreach (var line in lines)
        {
            if (line.StartsWith("p sp"))
            {
                var parts = line.Split(' ');
                vertices = int.Parse(parts[2]);
                edges = int.Parse(parts[3]);
                graph = new Graph(vertices);
            }
            else if (line.StartsWith("a"))
            {
                var parts = line.Split(' ');
                int u = int.Parse(parts[1]);
                int v = int.Parse(parts[2]);
                int weight = int.Parse(parts[3]);
                graph.AddEdge(u, v, weight);
            }
        }

        return graph;
    }
}
