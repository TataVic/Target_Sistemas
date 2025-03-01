using System;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using System.IO;

class Program
{
    static void Main()
    {
        CorrigirXmlSeNecessario("dados.xml");

        // 1) Cálculo da variável SOMA
        Console.WriteLine($"1) Valor da variável SOMA: {CalcularSoma()}");

        // 2) Verificação na sequência de Fibonacci
        Console.Write("2) Informe um número para verificar na sequência de Fibonacci: ");
        if (int.TryParse(Console.ReadLine(), out int num))
        {
            Console.WriteLine(VerificaFibonacci(num));
        }
        else
        {
            Console.WriteLine("Entrada inválida! Digite um número inteiro.");
        }

        // 3) Análise de faturamento diário
        Console.WriteLine("3) Analisando faturamento diário (JSON e XML)...");
        AnalisarFaturamentoJson("dados.json");
        AnalisarFaturamentoXml("dados.xml");

        // 4) Percentual de faturamento por estado
        Console.WriteLine("4) Percentual de faturamento por estado:");
        CalcularPercentualFaturamento();

        // 5) Inversão de string
        Console.Write("5) Informe uma string para inverter: ");
        string? input = Console.ReadLine();
        Console.WriteLine($"String invertida: {InverterString(input)}");
    }

    static int CalcularSoma()
    {
        int indice = 13, soma = 0, k = 0;
        while (k < indice)
        {
            k++;
            soma += k;
        }
        return soma;
    }

    static string VerificaFibonacci(int num)
    {
        if (num < 0) return "Número inválido para sequência de Fibonacci.";

        int a = 0, b = 1, temp;
        while (a < num)
        {
            temp = a + b;
            a = b;
            b = temp;
        }
        return a == num ? "O número pertence à sequência de Fibonacci." : "O número NÃO pertence à sequência de Fibonacci.";
    }

    static void AnalisarFaturamentoJson(string caminhoArquivo)
    {
        if (!File.Exists(caminhoArquivo))
        {
            Console.WriteLine("Arquivo JSON não encontrado.");
            return;
        }

        try
        {
            string json = File.ReadAllText(caminhoArquivo);
            var dados = JsonSerializer.Deserialize<DadosFaturamento[]>(json) ?? Array.Empty<DadosFaturamento>();
            var valores = dados.Where(d => d.Valor > 0).Select(d => d.Valor).ToList();

            if (!valores.Any())
            {
                Console.WriteLine("Sem dados disponíveis no JSON.");
                return;
            }

            Console.WriteLine($"Menor faturamento (JSON): {valores.Min():F2}");
            Console.WriteLine($"Maior faturamento (JSON): {valores.Max():F2}");
            Console.WriteLine($"Dias acima da média (JSON): {valores.Count(v => v > valores.Average())}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar JSON: {ex.Message}");
        }
    }

    static void AnalisarFaturamentoXml(string caminhoArquivo)
    {
        if (!File.Exists(caminhoArquivo))
        {
            Console.WriteLine("Arquivo XML não encontrado.");
            return;
        }

        try
        {
            var doc = XDocument.Load(caminhoArquivo);
            var valores = doc.Descendants("row")
                .Select(x => double.TryParse(x.Element("valor")?.Value, out double v) ? v : 0)
                .Where(v => v > 0)
                .ToList();

            if (!valores.Any())
            {
                Console.WriteLine("Sem dados disponíveis no XML.");
                return;
            }

            Console.WriteLine($"Menor faturamento (XML): {valores.Min():F2}");
            Console.WriteLine($"Maior faturamento (XML): {valores.Max():F2}");
            Console.WriteLine($"Dias acima da média (XML): {valores.Count(v => v > valores.Average())}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar XML: {ex.Message}");
        }
    }

    static void CalcularPercentualFaturamento()
    {
        var estados = new (string Estado, double Valor)[]
        {
            ("SP", 67836.43), ("RJ", 36678.66), ("MG", 29229.88), ("ES", 27165.48), ("Outros", 19849.53)
        };

        double total = estados.Sum(e => e.Valor);
        foreach (var (estado, valor) in estados)
        {
            Console.WriteLine($"{estado}: {(valor / total) * 100:F2}%");
        }
    }

    static string InverterString(string? input)
    {
        if (string.IsNullOrEmpty(input)) return "String vazia ou nula!";
        
        char[] caracteres = new char[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            caracteres[i] = input[input.Length - 1 - i];
        }
        return new string(caracteres);
    }

    static void CorrigirXmlSeNecessario(string caminhoArquivo)
    {
        if (!File.Exists(caminhoArquivo)) return;

        try
        {
            string conteudo = File.ReadAllText(caminhoArquivo).Trim();

            // Se o XML não começar com <root>, significa que precisa ser corrigido
            if (!conteudo.StartsWith("<root>"))
            {
                conteudo = "<root>\n" + conteudo + "\n</root>";
                File.WriteAllText(caminhoArquivo, conteudo);
                Console.WriteLine("O XML foi corrigido automaticamente.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao corrigir XML: {ex.Message}");
        }
    }
}

class DadosFaturamento
{
    public int Dia { get; set; }
    public double Valor { get; set; }
}
