using System;
using System.Threading;
using System.IO;

namespace Stack_To_The_Top_CONSOLEVERSION
{
    class Program
    {
        // --[GLOBÁLIS VÁLTOZÓK]--
        public static int[] t = { 1, 0, 0, 0 };
        public static int difficulty = 1;
        public static int[,] field = new int[10, 10];
        public static int char_length = 3;
        // \-----------------------------------------------------------------/

        // --[MENÜ RENDSZER]--
        static void arrow_move(int where)  //  A WHERE KETTŐ ÉRTÉK LEHET: 0 ÉS 1, HA 0, AKKOR LEFELE NAVIGÁLUNK. HA 1, AKKOR PEDIG FELFELE.  \\
        {
            int wich = 0;   // A WICH VÁLTOZÓ AZT FOGJA TÁROLNI, HOGY MELYIK MENÜPONTON ÁLTTUNK KORÁBBAN \\
            for (int i = 0; i < t.Length; i++)  //  EZT A "t" NEVŰ VEKTOR TÁROLJA. NÉGY MENÜPONT, NÉGY ÉRTÉK. AZ AKTÍV MENÜPONTÉ 1, A TÖBBIÉ NULLA.  \\
            {
                if (t[i] == 1)
                {
                    if (i == 0 && where == 1) { wich = 4; } //  HA A FELSŐ MENÜPONTON ÁLLUNK, ÉS FELFELE NAVIGÁLUNK AKKOR A WICH VÁLTOZÓ 4 \\
                    else if (i == 3 && where == 0) { wich = -1; } //  AZ ALSÓ MENÜPONT ESETÉBEN, HA LEFELE NAVIGÁLUNK, AKKOR -1  \\
                    else wich = i; //  KÜLÖNBEN PEDIG MEGKAPJA A WICH VÁLTOZÓ A MENÜPONT VALÓDI ÉRTÉKÉT. PL.: BEAÁLLÍTÁSOK - 3  \\
                }
            }
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = 0;
            }
            if (where == 1) t[wich - 1] = 1;
            else t[wich + 1] = 1;
        }

        static void arrow_draw()
        {
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] == 1) { Console.SetCursorPosition(15, i); Console.Write("<-"); break; }
            }
        }

        static void menu_pos()
        {
            var input = Console.ReadKey();

            switch (input.Key)
            {
                case ConsoleKey.UpArrow: //  FELFELE MUTATÓ NYÍL ESETÉN MEGHÍVJA AZ ARROW_MOVE FÜGGVÉNYT, ÉS 1 - ES ATTRIBÚTUMOT AD NEKI  \\
                    arrow_move(1);
                    arrow_draw();       //  MAJD MIUTÁN A FÜGGÉNY ELDÖNTÖTTE, HOGY MERRE MENJEN A NYÍL, KIRAJZOLJA AZT A PROGRAM ISMÉT  \\
                    break;
                case ConsoleKey.DownArrow: //  LEFELE MUTATÓ NYÍL ESETÉN A FÜGGVÉNY 0 - ÁS ATTRIBÚTUMOT KAP \\
                    arrow_move(0);
                    arrow_draw();
                    break;
                case ConsoleKey.Enter:  // ENTER BILLENTYŰ ESETÉNY A FELHASZNÁLÓ KIVÁLASZTOTTA A KÍVÁNT MENÜPONTOT\\ 
                    select_menu_item();      //  A PROGRAM A SELECT_MENU_ITEMBEN MEGNÉZI, HOGY MIT vÁLASZTOTTUNK  \\ 
                    break;
            }
        }

        static void select_menu_item()
        {
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] == 1)
                {
                    if (i == 0) game();
                    else if (i == 1) open_scoreboard();
                    else if (i == 2) open_options();
                    else if (i == 3) Environment.Exit(0);
                    break;
                }
            }
        }

        static void open_options()
        {
            Console.Clear();
            Console.WriteLine("Itt állíthatod be a játék nehézségét!");
            Console.Write("Számmal adhatod meg |1,2,3|"); Console.WriteLine("\t [Jelenlegi nehézség: " + difficulty + "]");
            Console.WriteLine("|1-es: Ez az alap nehézség, fél másodpercenként mozdul a 'stack' bábu.|");
            Console.WriteLine("|2-es: Randomizálódik a bábu mozgási sebessége.|");
            Console.WriteLine("|3-as: Szélesebb skálán randomizálódik a bábu sebessége.|");
            Console.WriteLine();
            int input = 0;
            while (input < 1 || input > 3)
            {
                do
                {
                    Console.Write("Add meg a nehézséget: ");
                } while (!int.TryParse(Console.ReadLine(), out input));
            }
            difficulty = input;
            Console.Clear();
            Console.WriteLine("Sikeresen átállítottad a nehézséget erre: " + difficulty);
            Console.WriteLine("Rögtön vissza kerülsz a főmenübe!");
            Thread.Sleep(4000);
        }

        static void open_scoreboard()
        {
            Console.Clear();
            if (File.Exists("saves.txt"))
            {
                StreamReader r = new StreamReader("saves.txt");
                Console.WriteLine("Név \t   Mikor \t\t Nehézség\t\t|");
                Console.WriteLine("--------------------------------------------------------");
                while (!r.EndOfStream)
                {
                    string[] line = r.ReadLine().Split('\t');
                    Console.WriteLine(line[0] + "\t" + line[1] + "\t" + line[2]);
                }
                r.Close();
            }
            else { Console.WriteLine("Még nincs mentett eredmény :("); }

            Console.ReadLine();
        }

        static void open_menu()  // A MAIN FÜGGVÉNY ŐT HÍVJA MEG A PROGRAM INDULÁSAKOR.  \\
        {
            while (true)
            {
                Console.WriteLine("1. Játék");
                Console.WriteLine("2. Scoreboard");
                Console.WriteLine("3. Beállítások");
                Console.WriteLine("4. Kilépés");
                arrow_draw();   //  EZ A FÜGGVÉNY RAJZOLJA KI A CONSOLRA A "<-" NYILAT, HOGY A FELHASZNÁLÓ LÁSSA, HOGY MELYIK MENÜPONT VAN KIVÁLASZTVA  \\
                menu_pos();    // EZ A FÜGGVÉNY FIGYELI A LEÜTÖTT BILLENTYŰT, A NYILAK, ÉS AZ ENTER GOMB KAPOTT SAJÁT FUNKCIÓT  \\ 
                Console.Clear();
            }
        }
        // MENÜ RENDSZER VÉGE \\
        // \-----------------------------------------------------------------/
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            open_menu();
        }

        // --[CORE GAMEPLAY SCRIPTJE]--
        static void game() // A "JÁTÉK" MENÜPONT KIVÁLASZTÁSA UTÁN FUT LE Ő  \\
        {
            static void field_upload()    // 10X10 ES TÖMBÖT FELTÖLT NULLÁKKAL, EZ LESZ A PÁLYA. AHOL KÉSÖBB 1-ES ÉRTÉK LESZ A MÁTRIXBAN, OTT LESZ MAG A BÁBU \\
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        field[i, j] = 0;
                    }
                }
            }

            static void item_move() // A PÁLYAFELTÖLST KÖVETŐEN EZ A FÜGGVÉNY FUT TOVÁBB, EGÉSZEN ADDIG NEM VESZÍT VAGY NYER A JÁTÉKOS  \\
            {
                Random r = new Random();
                int line = 1; // KÜLÖN VÁLTOZÓ TÁROLJA, HOGY MELYIK SORBAN JÁRUNK, A CIKLUS MAJD ÖSSZ SÓRSZÁMBÓL VONJA KI, ÍGY NEM KELLETT FORDÍTVA GONDOLKODNI SEHOL ^^

                for (int i = 0; i < 10; i++) 
                {
                    if (char_length > 0) // AMÍG VAN A BÁUNKBÓL..  \\
                    {
                        int counter = 0; // EZ A VÁLTOZÓ TÁROLJA, HOGY ADOTT SORBAN MELYIK OSZLOPBAN VAGYUNK. \\
                        do
                        {
                            while (!Console.KeyAvailable)
                            {
                                Console.Clear();    // TÖRÖLJÜK A CONSOLET FOLYTON \\
                                if (counter < 7) counter++; // HA 7. OSZLOPBAN JÁRUNK AKKOR A BÁBU MÁR A 8, ÉS 9. OSZLOPBA IS BELÓG, ÍGY ÚJRA SZÁMOLUNK 0-TÓL \\
                                else counter = 0;
                                draw_items(counter, line);  // KIRAJZOLJUK ÚJRA AZ EGÉSZ PÁLYÁT, MEG AHOL ÉPPEN JÁR A KARAKTERÜNK \\
                                if (difficulty == 1) Thread.Sleep(500); // ITT DÖL EL AZ EGÉSZNEK A SEBESSÉGE, A NEHÉZSÉGTŐL FÜGGŐEN \\
                                else if (difficulty == 2) Thread.Sleep(r.Next(400, 651));
                                else Thread.Sleep(r.Next(200, 701));
                            }
                        } while (Console.ReadKey(true).Key != ConsoleKey.Spacebar); // AMÍG NEM NYOM SPACET A JÁTÉKOS ADDIG MOZOG A BÁBU MEGÁLLÁS NÉLKÜL \\
                        lock_item(counter, line);   // EZ NÉZI MEG, HOGY SIKERÜLT-E JÓ HELYRE TENNI A BÁBUT, HA IGEN, AKKOR PEDIG MENTI A FIELD TÖMBEN \\
                        line++; // VÉGEZTÜNK AZ ADOTT SORRAL, MEGYÜNK TOVÁBB \\

                        //  Az alábbi kódsorok feloldásával lehet a pályát "debug" módban követni! enterrel állítja meg a debugot  \\

                        /*Console.SetCursorPosition(0, 13);
                        for (int x = 0; x < 10; x++)
                        {
                            for (int l = 0; l < 10; l++)
                            {
                                Console.Write(field[x, l]);
                            }
                            Console.WriteLine();
                        }
                        Console.WriteLine("char length:{0}", char_length);
                        Console.ReadLine();*/
                    }
                    else { Console.Clear(); Console.WriteLine("A játéknak vége! Elfogyott a bábud :("); Thread.Sleep(2800); break; }
                }
                if (char_length > 0)
                {
                    Console.Clear();
                    string nick = "";
                    do
                    {
                        Console.Write("Ügyes voltál! Adj meg egy nicknevet(max 7 karakter, min 2), ezen elmentjük a statisztikád: ");
                        nick = Console.ReadLine();
                    } while (nick.Length < 2 || nick.Length > 7);
                    string date = DateTime.Now.ToString("yyyy.MM.dd-HH:mm:s");
                    string dif = "";
                    if (difficulty == 1) dif = "Könnyű fokozat";
                    else if (difficulty == 2) dif = "Közepes fokozat";
                    else dif = "Nehéz fokozat";
                    StreamWriter w = new StreamWriter("saves.txt", append: true);
                    w.WriteLine(nick + "\t" + date + "\t" + dif);
                    w.Close();
                }
                char_length = 3;
            }

            static void draw_items(int counter, int sor)
            {
                for (int i = 0; i < 10; i++) // Ez a ciklus rajolja ki a teljes eddigi pályát \\
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (field[i, j] == 1) { Console.SetCursorPosition(j, i); Console.Write("█"); }
                    }
                }

                for (int i = 0; i < char_length; i++) // Ez pedig, hogy éppen hol a karakter. \\
                {
                    Console.SetCursorPosition(counter + i, 10 - sor);
                    Console.Write('█');
                }
            }

            static void lock_item(int counter, int sor)
            {
                int prev_line = sor - 1;
                int errors = 0;
                for (int i = 0; i < char_length; i++)
                {
                    if (sor > 1)
                    {
                        if (field[10 - prev_line, counter + i] == 1) { field[10 - sor, counter + i] = 1; } //Az eldöntés, hogy jó helyre akarjuk-e tenni a bábut\\
                        else { errors++; field[10 - sor, counter + i] = 0; }
                    }
                    else field[10 - sor, counter + i] = 1;
                }
                char_length -= errors;
            }

            field_upload();
            item_move();
        }
        //  ITT A VÉGE A CORE GAMEPLAY SCRIPTNEK \\
        // \-----------------------------------------------------------------/
    }
}
