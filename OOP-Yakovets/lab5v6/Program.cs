using System;
using System.Collections.Generic;

namespace Lab5_Polymorphism
{
    public class Dish
    {
        public string Name { get; set; }

        public Dish(string name)
        {
            Name = name;
        }

        public virtual void Prepare()
        {
            Console.WriteLine($"[Dish] Готується загальна страва: {Name}");
        }
    }

    public class Pizza : Dish
    {
        public string Toppings { get; set; }

        public Pizza(string name, string toppings) : base(name)
        {
            Toppings = toppings;
        }

        public override void Prepare()
        {
            Console.WriteLine($"[Pizza] Випікається піца '{Name}' з начинкою: {Toppings}");
        }
    }

    public class Soup : Dish
    {
        public string BrothType { get; set; }

        public Soup(string name, string brothType) : base(name)
        {
            BrothType = brothType;
        }

        public override void Prepare()
        {
            Console.WriteLine($"[Soup] Вариться суп '{Name}' на бульйоні: {BrothType}");
        }
    }

    public class Salad : Dish
    {
        public string Dressing { get; set; }

        public Salad(string name, string dressing) : base(name)
        {
            Dressing = dressing;
        }

        public override void Prepare()
        {
            Console.WriteLine($"[Salad] Нарізається та заправляється салат '{Name}' соусом: {Dressing}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Dish> menu = new List<Dish>
            {
                new Pizza("Маргарита", "Моцарела, соус томатний, базилік"),
                new Soup("Борщ український", "М'ясний (яловичина)"),
                new Salad("Цезар", "Пармезан, сухарики, соус Цезар"),
                new Pizza("Пепероні", "Салямі пепероні, сир моцарела"),
                new Salad("Грецький", "Оливкова олія, сир Фета")
            };

            Console.WriteLine("=== ДЕМОНСТРАЦІЯ ПОЛІМОРФІЗМУ (ГОТУВАННЯ СТРАВ) ===");
            
            List<string> preparedDishes = new List<string>();

            foreach (var dish in menu)
            {
                dish.Prepare(); 
                preparedDishes.Add(dish.Name); 
            }

            Console.WriteLine("\nАгрегація результатів");
            Console.WriteLine($"Усього приготовано страв: {preparedDishes.Count}");
            Console.WriteLine("Список усіх приготованих страв:");
            foreach (var dishName in preparedDishes)
            {
                Console.WriteLine($" - {dishName}");
            }
        }
    }
}