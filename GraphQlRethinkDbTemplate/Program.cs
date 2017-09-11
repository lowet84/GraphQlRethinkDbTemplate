﻿using System.IO;
using GraphQlRethinkDbTemplate.Database;
using GraphQlRethinkDbTemplate.Schema.Model;
using Microsoft.AspNetCore.Hosting;

namespace GraphQlRethinkDbTemplate
{
    public class Program
    {
        private const bool Reset = true;

        public static void Main(string[] args)
        {
            var dbContext = DbContext.Instance;
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls("http://*:7000")
                .Build();
            Init();
            host.Run();
        }

        public static void Init()
        {
            if (Reset)
            {
                DbContext.Instance.Reset();

                var author = new Author("Axel", "Axelsson");
                var book = new Book("En bok", author);
                var series = new Series("En serie böcker", null);
                var newSeries = new Series(series.Name, new[] { book });

                DbContext.Instance.AddDefault(author);
                DbContext.Instance.AddDefault(book);
                DbContext.Instance.AddDefault(series);
                DbContext.Instance.AddDefault(newSeries, series.Id);

                DbContext.Instance.Test(series.Id);
            }
        }
    }
}