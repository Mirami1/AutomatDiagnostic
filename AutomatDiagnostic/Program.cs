using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Text;
using static System.Console;

namespace AutomatDiagnostic
{
    class Program
    {
        [Serializable]
        class Q
        {
            public int number { get; set; }
            public List<int> path { get; set; }
            public List<int> bs { get; set; }

            public Q(int _number)
            {
                number = _number;
                path = new List<int>();
                bs = new List<int>();
            }

            public void defenitePAth(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    WriteLine("При a = {0} переход из Q{1}  в: ", i, number);
                    path.Add(Convert.ToInt32(ReadLine()));
                    WriteLine("При a = {0} на переходе из Q{1} в Q{2} на выход подается: ", i, number, path[i]);
                    bs.Add(Convert.ToInt32(ReadLine()));
                }
            }

            public ValueTuple<int, int> GetQandB(int a) => (path[a], bs[a]);
        }

        static void Main(string[] args)
        {
            Dictionary<char, dynamic> save_dict = new Dictionary<char, dynamic>();
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var fi = new System.IO.FileInfo(@"save.bin");

            bool load = false;
            List<Q> q_states = new List<Q>();
            int q_state = 0;
            if (!load)
            {
                WriteLine("Введите количество состояний q:");
                q_state = Convert.ToInt32(ReadLine());
                for (int i = 0; i < q_state; i++)
                {
                    q_states.Add(new Q(i + 1));
                    q_states[i].defenitePAth(3);
                }

                save_dict.Add('0', q_states);
                save_dict.Add('1', q_state);


                using (var binaryFile = fi.Create())
                {
                    binaryFormatter.Serialize(binaryFile, save_dict);
                    binaryFile.Flush();
                }
            }
            else
            {
                using (var binaryFile = fi.OpenRead())
                {
                    save_dict = (Dictionary<char, dynamic>) binaryFormatter.Deserialize(binaryFile);
                    q_states = save_dict['0'];
                    q_state = save_dict['1'];
                }
            }

            int kf = Convert.ToInt32(Math.Ceiling(Math.Log(q_state, 2)));
          
            while (true)
            {
                bool flag = false;

                int[] a = new int[kf];
                for (int i = 0; i < Convert.ToInt32(Math.Pow(3, kf)); i++)
                {
                     flag = false;
                    int i_cch = i; //число для перевода в двочиное представление
                    for (int bj = 1; bj < kf + 1; bj++)
                    {
                        a[bj - 1] = Convert.ToInt32(Math.Floor(i_cch / Math.Pow(3, kf - bj)));
                        i_cch = i_cch % Convert.ToInt32(Math.Pow(3, kf - bj));
                    }

                    List<StringBuilder> bstrings = new List<StringBuilder>();
                    for (int j = 0; j < q_state; j++)
                        bstrings.Add(new StringBuilder(q_state));
                    bstrings.Select((bs) => bs.Capacity = q_state);

                    for (int qi = 0; qi < q_state; qi++)
                    {
                        Q current_Q = q_states[qi];
                        for (int k = 0; k < kf; k++)
                        {
                            (int next_q, int b) = current_Q.GetQandB(a[k]);
                            current_Q = q_states[next_q - 1];
                            bstrings[qi] = bstrings[qi].Append(b);
                        }
                    }

                    for (int jr = 0; jr < q_state; jr++)
                    {
                        for (int j = jr + 1; j < q_state; j++)
                        {
                            if (bstrings[jr].ToString() == bstrings[j].ToString())
                            {
                                flag = true;
                                break;
                            }
                        }

                        if (flag) break;
                    }

                    if (flag) continue;
                    else
                    {
                        WriteLine(String.Join(",", a));
                        WriteLine(String.Join(",", bstrings));
                        ReadKey();
                    }
                }
                break;
            }
        }
    }
}