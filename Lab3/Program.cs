using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    class Program
    {
        public static List<Objects> O;
        public static List<Subjects> S;
        public static List<Edges> E;
        public static List<Edges> F;
        public static List<Objects> O0;
        public static List<Subjects> S0;
        public static List<Edges> E0;
        public static List<Edges> F0;
        //public static string currentuser;
        //public static int ID;
        public class Objects
        {
            public string name;

            public Objects(string n)
            {
                this.name = n;
            }
        }
        public class Subjects
        {
            public string name;
            public Subjects (string n)
            {
                this.name = n;
            }
        }
        public class Edges
        {
            public string from;
            public string to;
            public string rights;

            public Edges (string f, string t, string r)
            {
                this.from = f;
                this.to = t;
                this.rights = r;
            }
        }
        public static void Initialization()
        {
            S0 = new List<Subjects>();
            O0 = new List<Objects>();
            E0 = new List<Edges>();
            F0 = new List<Edges>();
            //currentuser = ""; ID = 0;

            //конфигурация
            S0.Add(new Subjects("x1"));

            O0.Add(new Objects("x1"));
            O0.Add(new Objects("y1"));
            O0.Add(new Objects("y2"));
            O0.Add(new Objects("z1"));
            O0.Add(new Objects("z2"));
            O0.Add(new Objects("z3"));

            E0.Add(new Edges("x1", "y1", "tg"));
            E0.Add(new Edges("x1", "y2", "t"));
            E0.Add(new Edges("y1", "z1", "w"));
            E0.Add(new Edges("y2", "z2", "r"));
            E0.Add(new Edges("z2", "z3", "r"));
        }
        public static void Menu()
        {
            Console.WriteLine("Граф:");
            foreach (Edges e0 in E0)
                Console.WriteLine("{0}\t{1}\t{2}", e0.from,e0.to,e0.rights);

            Console.WriteLine("\nВыберите действие: ");
            Console.WriteLine("1: Де-юре операции");
            Console.WriteLine("2: Проверить предикат");
            Console.WriteLine("----------------------\n");
            Console.Write(">\t");

            string k = Console.ReadLine();
            switch (k)
            {
                case "1": DeJure(); break;
                case "2": Predicates(); break;
                default:
                    Console.WriteLine("------------------------------------------------");
                    Console.WriteLine("Ошибка! Выберите один из предлагаемых вариантов!");
                    Console.WriteLine("------------------------------------------------\n");
                    Menu(); break;
            }
        }      
        public static void DeJure()
        {
            Console.WriteLine("---------------------------\n");
            Console.WriteLine("Выберите де-юре операцию:");
            Console.WriteLine("1: create");
            Console.WriteLine("2: remove");
            Console.WriteLine("3: отмена\n");
            Console.Write(">\t");

            string k = Console.ReadLine();
            switch (k)
            {
                case "1": 
                    create();
                    break;
                case "2": 
                    remove();
                    break;
                case "3": Menu(); break;
                default:
                    Console.WriteLine("------------------------------------------------");
                    Console.WriteLine("Ошибка! Выберите один из предлагаемых вариантов!");
                    Console.WriteLine("------------------------------------------------\n");
                    Menu(); break;
            }
        }
        public static void create()
        {
            Console.WriteLine("\n--------------------");
            Console.WriteLine("Субъект или объект?");
            Console.WriteLine("1: субъект\n2: объект\n>\t");
            string k = Console.ReadLine();
            Console.Write("\nВведите имя: ");
            string name = Console.ReadLine();
            if (O0.Exists(q => q.name == name))
            {
                Console.WriteLine("\nОшибка! Такой элемент уже существует!");
                create();
            }
            else
            {
                Console.Write("\nВыберите права: ");
                string rights = Console.ReadLine();
                Console.Write("\nЧьи это права: ");
                string from = Console.ReadLine();
                if (!O0.Exists(q => q.name == from))
                {
                    Console.WriteLine("\nОшибка! Такой элемент не существует!");
                    create();
                }
                else
                {
                    switch (k)
                    {
                        case "1":
                            S0.Add(new Subjects(name));
                            O0.Add(new Objects(name));
                            E0.Add(new Edges(from, name, rights));
                            break;
                        case "2":
                            O0.Add(new Objects(name));
                            E0.Add(new Edges(from, name, rights));
                            break;
                        default:
                            Console.WriteLine("------------------------------------------------");
                            Console.WriteLine("Ошибка! Выберите один из предлагаемых вариантов!");
                            Console.WriteLine("------------------------------------------------\n");
                            create(); break;
                    }
                }
            }
            tg_closure();
        }
        public static void remove()
        {
            Console.WriteLine("\n------------------");
            Console.Write("x: ");
            string x = Console.ReadLine();
            Console.Write("y: ");
            string y = Console.ReadLine();
            Console.Write("a: ");
            string a = Console.ReadLine();

            int index = E0.FindIndex(q => q.from == x && q.to == y);
            if (index!=-1)
            {
                char[] rights = a.ToArray();
                foreach (char c in rights)
                    if (!E0[index].rights.Contains(c))
                        continue;
                    else
                    {
                        string r = E0[index].rights.Replace(c.ToString(), "");
                        E0.RemoveAt(index);
                        E0.Add(new Edges(x, y, r));
                    }
            }
            else remove();
            tg_closure();
        }

        #region де-факто правила
        public static Edges first_rule(string x, string y)
        {
            //если x читает y, то у пишет в х
            List<Edges> EF = E.Concat(F).ToList();
            if (EF.Exists(q => (q.from == x && q.to == y && q.rights.Contains("r"))))
                return new Edges(y, x, "w");
            else return new Edges("", "", "");
            
        }
        public static Edges second_rule(string x, string y)
        {
            //если х пишет в у, то у читает х 
            List<Edges> EF = E.Concat(F).ToList();
            if (EF.Exists(q => (q.from == x && q.to == y && q.rights.Contains("w"))))
                return new Edges(y, x, "r");
            else return new Edges("", "", "");
        }
        public static void spy(string x, string y, string z, ref List<Edges>L)
        {
            /* если х читает у, а у читает z
            то х читает z, a z пишет в х */
            List<Edges> EF = E.Concat(F).ToList();
            if (EF.Exists(q => (q.from == x && q.to == y && q.rights.Contains("r"))))
            {
                if (EF.Exists(p => (p.from == y && p.to == z && p.rights.Contains("w"))))
                {
                    F.Add(new Edges(x, z, "r")); F.Add(new Edges(z, x, "w"));
                    L.Add(new Edges(x, z, "r")); L.Add(new Edges(z, x, "w"));
                }
            }
        }
        public static void find(string x, string y, string z, ref List<Edges> L)
        {
            /* если х пишет в у, а у пишет в z
            то х пишет в z, a z читает х */
            List<Edges> EF = E.Concat(F).ToList();
            if (EF.Exists(q => (q.from == x && q.to == y && q.rights.Contains("w"))))
            {
                if (EF.Exists(p => (p.from == y && p.to == z && p.rights.Contains("w"))))
                {
                    F.Add(new Edges(x, z, "w")); F.Add(new Edges(z, x, "r"));
                    L.Add(new Edges(x, z, "w")); L.Add(new Edges(z, x, "r"));
                }
            }
        }
        public static void post(string x, string y, string z, ref List<Edges> L)
        {
            /* если х читает z, а у пишет в z
            то х читает y, a y пишет в х */
            List<Edges> EF = E.Concat(F).ToList();
            if (EF.Exists(q => (q.from == x && q.to == z && q.rights.Contains("r"))))
            {
                if (EF.Exists(p => (p.from == y && p.to == z && p.rights.Contains("w"))))
                {
                    F.Add(new Edges(x, y, "r")); F.Add(new Edges(y, x, "w"));
                    L.Add(new Edges(x, y, "r")); L.Add(new Edges(y, x, "w"));
                }
            }
        }
        public static void pass(string x, string y, string z, ref List<Edges> L)
        {
            /* если х пишет в у и читает z
            то z пишет в у, a у читает z */
            List<Edges> EF = E.Concat(F).ToList();
            if (EF.Exists(q => (q.from == x && q.to == y && q.rights.Contains("w"))))
            {
                if (EF.Exists(p => (p.from == x && p.to == z && p.rights.Contains("r"))))
                {
                    F.Add(new Edges(z, y, "w")); F.Add(new Edges(y, z, "r"));
                    L.Add(new Edges(z, y, "w")); L.Add(new Edges(y, z, "r"));
                }
            }
        }
        #endregion

        #region предикаты
        public static void Predicates()
        {
            Console.WriteLine("\nВыберите предикат для проверки:");
            Console.WriteLine("1: can_share");
            Console.WriteLine("2: can_steal");
            Console.WriteLine("3: can_write");
            Console.WriteLine("4: отмена\n");
            Console.Write(">\t");
            string k = Console.ReadLine();

            Console.WriteLine("\n------------------");
            Console.Write("x: ");
            string x = Console.ReadLine();
            Console.Write("y: ");
            string y = Console.ReadLine();
            
            switch (k)
            {
                case "1":
                    Console.Write("a: ");
                    string alpha = Console.ReadLine();
                    if (access_possible(alpha,x,y)) 
                    {
                        Console.WriteLine("\n---------------");
                        Console.WriteLine("Истина!");
                        Console.WriteLine("---------------\n");
                    }
                    else
                    {
                        Console.WriteLine("\n---------------");
                        Console.WriteLine("Ложь!");
                        Console.WriteLine("---------------\n");
                    }
                    Menu();
                    break;
                case "2":
                    Console.Write("a: ");
                    alpha = Console.ReadLine();
                    if (steal_possible(alpha,x,y))
                    {
                        Console.WriteLine("\n---------------");
                        Console.WriteLine("Истина!");
                        Console.WriteLine("---------------\n");
                    }
                    else
                    {
                        Console.WriteLine("\n---------------");
                        Console.WriteLine("Ложь!");
                        Console.WriteLine("---------------\n");
                    }
                    Menu();
                    break;
                case "3":
                    if (write_possible(x,y))
                    {
                        Console.WriteLine("\n---------------");
                        Console.WriteLine("Истина!");
                        Console.WriteLine("---------------\n");
                    }
                    else
                    {
                        Console.WriteLine("\n---------------");
                        Console.WriteLine("Ложь!");
                        Console.WriteLine("---------------\n");
                    }
                    Menu();
                    break;
                case "4": break;
                default:
                    Console.WriteLine("------------------------------------------------");
                    Console.WriteLine("Ошибка! Выберите один из предлагаемых вариантов!");
                    Console.WriteLine("------------------------------------------------\n");
                    Menu(); break;
            }
        }
        public static bool access_possible(string alpha, string x, string y)
        {
            char[] r = alpha.ToArray();
            bool flag = true;
            int index = E.FindIndex((q => (q.from == x && q.to == y)));
            if (index != -1)
            {
                foreach (char c in r)
                    if (!E[index].rights.Contains(c))
                        return false;
                if (flag)
                    return true;
                else return false;
            }
            else return false;
        }
        public static bool steal_possible(string alpha, string x, string y)
        {            
            char[] r = alpha.ToArray();
            int index = E0.FindIndex((q => (q.from == x && q.to == y)));
            if (index == -1)
            {
                int ind = E.FindIndex((q => (q.from == x && q.to == y)));
                if (ind != -1)
                {
                    bool flag = true;
                    foreach (char c in r)
                        if (!E[ind].rights.Contains(c))
                            return false;
                    if (flag)
                        return true;
                    else return false;
                }
                else return true;
            }
            else return false;
        }
        public static bool write_possible(string x, string y)
        {
            int index = F.FindIndex((q => (q.from == x && q.to == y&&q.rights.Contains("w"))));
            if (index != -1)          
                return true;
            else return false;
        }
        #endregion

        #region замыкания
        public static void tg_closure()
        {
            S = new List<Subjects>();
            O = new List<Objects>();
            E = new List<Edges>();
            F = new List<Edges>();
            foreach (Subjects s in S0)
                S.Add(s);
            foreach (Objects o in O0)
                O.Add(o);
            foreach (Edges e in E0)
                E.Add(e);

            Console.WriteLine("Строим замыкание...");
            foreach (Subjects s in S)
                create_object(s.name, "tg");

            List<Edges> L = new List<Edges>();
            foreach (Edges e in E)
            {
                if (e.rights.Contains("t") || e.rights.Contains("g"))
                    L.Add(new Edges(e.from, e.to, e.rights));
            }

            List<Objects> N = new List<Objects>();
            N.Add(new Objects(L[0].from));
            N.Add(new Objects(L[0].to));
            N.Add(new Objects(L[1].from));
            N.Add(new Objects(L[1].to));

            //N=N.Distinct().ToList();
            while(L.Count!=0)
            {
                N.Add(new Objects(L[0].from));
                N.Add(new Objects(L[0].to));
                string alpha = L[0].rights;
                N = DistinctObjects(N);
                IEnumerable<Objects> _N = N.Distinct();
                foreach (Objects n in _N)
                {
                    if (L[0].from != n.name && L[0].to != n.name)
                    {                       
                        Edges et = take_operation(alpha, L[0].from, L[0].to, n.name);
                        Edges eg = grant_operation(alpha, L[0].from, L[0].to, n.name);
                        if (et.from!=""&&et.to!="")
                        {
                            if (!E.Exists(q => (q.from == et.from && q.to == et.to && q.rights.Contains(et.rights))))
                            {
                                L.Add(et); E.Add(et); L = DistinctList(L);
                            }
                        }
                        if (!E.Exists(q => (q.from == eg.from && q.to == eg.to && q.rights.Contains(eg.rights))))
                        {
                            if (!E.Contains(eg))
                            {
                                L.Add(eg); E.Add(eg); L = DistinctList(L);
                            }
                        }
                        //L=L.Distinct().ToList();
                    }
                }
                L = DistinctList(L);
                L.Remove(L[0]);
                
            }
            de_jure_closure();
        }
        public static List<Edges>DistinctList(List<Edges>L)
        {
            List<Edges> T = new List<Edges>();
            for (int i=0; i<L.Count;i++)
            {
                if (!T.Exists(q => (q.from == L[i].from && q.to == L[i].to && q.rights.Contains(L[i].rights))))
                    T.Add(L[i]);
            }
            return T;
        }
        public static List<Objects> DistinctObjects(List<Objects> N)
        {
            List<Objects> T = new List<Objects>();
            for (int i = 0; i < N.Count; i++)
            {
                if (!T.Exists(q => (q.name == N[i].name)))
                    T.Add(N[i]);
            }
            return T;
        }
        public static void create_object(string x, string beta)
        {
            Console.Write("Введите имя: ");
            string y = Console.ReadLine();
            if (O.Exists(q=>q.name==y))
            {
                Console.WriteLine("\nОшибка! Такой элемент уже существует!");
                create();
            }
            else
            {
                O.Add(new Objects(y));
                E.Add(new Edges(x, y, beta));
            }
        }
        public static Edges grant_operation(string alpha, string x, string y, string z)
        {           
            if (E.Exists(q => (q.from == x && q.to == y && q.rights.Contains("g"))))
            {
                if (E.Exists(p => (p.from == x && p.to == z)))
                {
                    int index = E.FindIndex(p => (p.from == x && p.to == z));
                    bool flag=true;
                    foreach(char a in alpha) //альфа - подмножество бета
                    {
                        if (!E[index].rights.Contains(a))
                        {
                            flag=false;
                            break;
                        }
                    }
                    if(flag)
                        return new Edges(y, z, alpha);
                    else return new Edges("", "", "");
                }
                else return new Edges("", "", "");                
            }
            else return new Edges("", "", "");
        }
        public static Edges take_operation(string alpha, string x, string y, string z)
        {
            if (E.Exists(q => (q.from == x && q.to == y && q.rights.Contains("t"))))
            { 
                if (E.Exists(p => (p.from == y && p.to == z)))
                {
                    int index = E.FindIndex(p => (p.from == y && p.to == z));
                    bool flag=true;
                    foreach(char a in alpha) //альфа - подмножество бета
                    {
                        if (!E[index].rights.Contains(a))
                        {
                            flag=false;
                            break;
                        }
                    }
                    if (flag)
                        return new Edges(x, z, alpha);
                    else return new Edges("", "", "");
                }
                else return new Edges("", "", "");          
            }
            else return new Edges("", "", "");
        }
        public static void de_jure_closure()
        {
            int count = 1;
            List<Edges> temp = new List<Edges>();
            while (count <= 3) //выполняем в три шага
            {
                foreach (Subjects x in S) //для каждого субъекта x
                {                    
                    foreach (Edges e in E)
                    {
                        string takegrant = "";
                        if (count % 2 != 0) //если шаг 1 или 3, то take, если шаг 2, то grant
                            takegrant = "t";
                        else
                            takegrant = "g";
                        if (e.from == x.name && e.rights.Contains(takegrant)) //смотрим ребра, идущие из x, и помеченные как t или g
                        {
                            foreach (Objects y in O)
                            {
                                if (e.to == y.name) //смотрим все y для x
                                {
                                    foreach (Objects z in O)
                                    {
                                        if(takegrant=="t")
                                        { 
                                            if (z.name != x.name && z.name != y.name) //смотрим все потенциальные z, отличные от x и y
                                            {
                                                int index = E.FindIndex(p => (p.from == y.name && p.to == z.name));
                                                if (index == -1)
                                                    continue;
                                                else
                                                {
                                                    string alpha = E[index].rights;
                                                    Edges et = take_operation(alpha, x.name, y.name, z.name);
                                                    if ((et.from != "" && et.to != "") && !E.Contains(et))
                                                           temp.Add(et);
                                                }
                                            }
                                        }
                                        if (takegrant == "g")
                                        {
                                            if (z.name != x.name && z.name != y.name) //смотрим все потенциальные z, отличные от x и y
                                            {
                                                int index = E.FindIndex(p => (p.from == x.name && p.to == z.name));
                                                if (index == -1)
                                                    continue;
                                                else
                                                {
                                                    string alpha = E[index].rights;
                                                    Edges eg = grant_operation(alpha, x.name, y.name, z.name);
                                                    if ((eg.from != "" && eg.to != "")&&!E.Contains(eg))
                                                        temp.Add(eg);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                count++;
            }
            foreach (Edges t in temp)
                E.Add(t);
            de_facto_closure();
        }
        public static void de_facto_closure()
        {
            List<Edges> L = E.Concat(F).ToList();
            List<Edges> temp = new List<Edges>();
            foreach (Edges e in L)
                if ((!e.rights.Contains("w") && !e.rights.Contains("r"))&&!S.Contains(new Subjects(e.from)))
                    temp.Add(e);
            foreach (Edges t in temp)
                L.Remove(t);

            temp = new List<Edges>();
            foreach(Edges l in L) //применение первых двух де-факто правил
            {
                if (S.Exists(q => (q.name == l.from)))
                {
                    Edges first = first_rule(l.from, l.to);
                    Edges second = second_rule(l.from, l.to);
                    if (first.from != "" && first.to != "")
                    {
                        temp.Add(first); F.Add(first);
                    }
                    if (second.from != "" && second.to != "")
                    {
                        temp.Add(second); F.Add(second);
                    }
                    temp.Add(l);
                }
            }

            foreach (Edges l in L)
                temp.Add(l);

            List<Objects> N = new List<Objects>();
            N.Add(new Objects(temp[0].from));
            N.Add(new Objects(temp[0].to));
            N.Add(new Objects(temp[1].from));
            N.Add(new Objects(temp[1].to));
            //N=N.Distinct().ToList();
            while (temp.Count != 0)
            {
                N.Add(new Objects(temp[0].from));
                N.Add(new Objects(temp[0].to));
                string alpha = temp[0].rights;
                //N = N.Distinct().ToList();
                N = DistinctObjects(N);
                foreach (Objects n in N)
                {
                    if (temp[0].from != n.name && temp[0].to != n.name)
                    {
                        //E.Exists(q => (q.from == eg.from && q.to == eg.to && q.rights.Contains(eg.rights)))
                        if (S.Exists(q => (q.name == temp[0].from)) &&S.Exists(p => (p.name == temp[0].to)))
                        {
                            spy(temp[0].from, temp[0].to, n.name, ref temp);
                            find(temp[0].from, temp[0].to, n.name, ref temp);
                            post(temp[0].from, temp[0].to, n.name, ref temp);
                        }
                    }
                    if (S.Exists(q => (q.name == temp[0].from)))
                    {
                        pass(temp[0].from, temp[0].to, n.name, ref temp);
                    }
                }
                temp.Remove(temp[0]);
            }

            foreach (Edges f in F)
                E.Add(f);
            PrintGraph();
        }
        #endregion
        public static void PrintGraph()
        {
            //List<Edges> L = E.Concat(F).ToList();
            string[] rights = new string[] { "t", "g", "r", "w" };
            List<Edges> temp = new List<Edges>();
            List<Edges> L = new List<Edges>();
            foreach (Edges e in E)
            {
                temp = E.FindAll(p => (p.from == e.from && p.to == e.to));
                bool flag = L.Exists(q => (q.from == e.from && q.to == e.to));
                if (temp.Count!=0&&!flag)
                {
                    string r = "";
                    foreach (Edges t in temp)
                        r += t.rights;
                    string edge = "";
                    foreach (string rgt in rights)
                        if (r.Contains(rgt))
                            edge += rgt;
                    L.Add(new Edges(e.from, e.to, edge));
                }
            }
            //L.Add(new Edges("", "", ""));
            foreach (Edges f in F)
            {
                temp = F.FindAll(p => (p.from == f.from && p.to == f.to));
                bool flag = L.Exists(q => (q.from == f.from && q.to == f.to));
                if (temp.Count != 0&&!flag)
                {
                    string r = "";
                    foreach (Edges t in temp)
                        r += t.rights;
                    string edge = "";
                    foreach (string rgt in rights)
                        if (r.Contains(rgt))
                            edge += rgt;
                    L.Add(new Edges(f.from, f.to, edge));
                }

            }
            foreach (Edges l in L)
                Console.WriteLine("{0}\t{1}\t{2}", l.from, l.to, l.rights);
        }
        static void Main(string[] args)
        {
            Initialization();
            //PrintGraph();
            tg_closure();
            Menu();
        }
    }
}
