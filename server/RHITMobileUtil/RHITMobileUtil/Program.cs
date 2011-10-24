using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;

namespace RHITMobileUtil
{
    public class Program
    {
        private static int exec = 0;
        public static string upfile = @"C:\Users\glowskst\Desktop\Locations\Logan.txt";
        public static string trifile = @"C:\Users\glowskst\Desktop\Locations\Logan3.bmp";

        [STAThreadAttribute]
        public static void Main(string[] args)
        {
            new List<Action>()
            {
                UpdateLocations,
                Triangulate,
            }[exec]();
        }

        public static void Triangulate()
        {
            using (var form = new Trilaterator())
            {
                form.ShowDialog();
            }
        }

        public static void UpdateLocations()
        {
            string connectionStr = @"Data Source=mobilewin.csse.rose-hulman.edu\RHITMobile;Initial Catalog=MapData;User Id=admin;Password=rhitMobile56";

            string filepath = upfile;

            string line;

            using (var fileReader = new StreamReader(filepath))
            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                SqlCommand command;
                SqlDataReader reader;
                while ((line = fileReader.ReadLine()) != null)
                {
                    int commapos = line.IndexOf(',');
                    int locationid = Int32.Parse(line.Substring(0, commapos));
                    line = line.Substring(commapos + 1);

                    commapos = line.IndexOf(',');
                    string name = line.Substring(0, commapos);
                    int parentInt;
                    int? parent = null;
                    if (Int32.TryParse(line.Substring(commapos + 1), out parentInt))
                        parent = parentInt;

                    line = fileReader.ReadLine();

                    List<string> altnames = new List<string>();
                    while (line.Length > 4 && line.Substring(0, 4) == "alt:")
                    {
                        altnames.Add(line.Substring(4));
                        line = fileReader.ReadLine();
                    }

                    string description = line;

                    line = fileReader.ReadLine();

                    commapos = line.IndexOf(',');
                    int isPOI = Int32.Parse(line.Substring(0, commapos));
                    int onQuickList = Int32.Parse(line.Substring(commapos + 1));

                    line = fileReader.ReadLine();

                    commapos = line.IndexOf(',');
                    double lat = Double.Parse(line.Substring(0, commapos));
                    line = line.Substring(commapos + 1);

                    commapos = line.IndexOf(',');
                    double lon = Double.Parse(line.Substring(0, commapos));
                    line = line.Substring(commapos + 1);

                    int? labelOnHybrid = null;
                    int? minZoomLevel = null;
                    bool isMapArea = false;
                    commapos = line.IndexOf(',');
                    if (commapos >= 0)
                    {
                        isMapArea = true;
                        labelOnHybrid = Int32.Parse(line.Substring(0, commapos));
                        minZoomLevel = Int32.Parse(line.Substring(commapos + 1));
                    }

                    command = connection.CreateCommand();
                    command.CommandText = String.Format("SELECT id FROM Location WHERE id = {0}", locationid);
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Close();
                        command = connection.CreateCommand();
                        command.CommandText = String.Format("DELETE FROM MapAreaCorner WHERE area = {0}", locationid);
                        command.ExecuteNonQuery();
                        command = connection.CreateCommand();
                        command.CommandText = String.Format("UPDATE Location SET name = '{1}', description = '{2}', lat = {3}, lon = {4}, labelonhybrid = {5}, minzoomlevel = {6}, parent = {7}, ispoi = {8}, onquicklist = {9}, istop = 0 WHERE id = {0}", locationid, name, description, lat, lon, labelOnHybrid ?? (object)"NULL", minZoomLevel ?? (object)"NULL", parent ?? (object)"NULL", isPOI, onQuickList);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        reader.Close();
                        command = connection.CreateCommand();
                        command.CommandText = String.Format("DELETE FROM MapAreaCorner WHERE area = {0}", locationid);
                        command.ExecuteNonQuery();
                        command = connection.CreateCommand();
                        command.CommandText = String.Format("INSERT INTO Location VALUES({0}, '{1}', {2}, {3}, {4}, {5}, {6}, '{7}', {8}, {9}, 0)", locationid, name, lat, lon, parent ?? (object)"NULL", isPOI, onQuickList, description, labelOnHybrid ?? (object)"NULL", minZoomLevel ?? (object)"NULL");
                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine(locationid);
                    foreach (string altname in altnames)
                    {
                        command = connection.CreateCommand();
                        command.CommandText = String.Format("INSERT INTO LocationAltName VALUES('{0}', {1})", altname, locationid);
                        command.ExecuteNonQuery();
                    }
                    if (isMapArea)
                    {
                        while (!String.IsNullOrEmpty(line = fileReader.ReadLine()))
                        {
                            commapos = line.IndexOf(',');
                            int cornerid = Int32.Parse(line.Substring(0, commapos));
                            line = line.Substring(commapos + 1);
                            commapos = line.IndexOf(',');
                            lat = Double.Parse(line.Substring(0, commapos));
                            lon = Double.Parse(line.Substring(commapos + 1));
                            command = connection.CreateCommand();
                            command.CommandText = String.Format("INSERT INTO MapAreaCorner VALUES({0}, {1}, {2}, {3})", cornerid, locationid, lat, lon);
                            command.ExecuteNonQuery();
                            Console.WriteLine("{0}: {1}", locationid, cornerid);
                        }
                    }
                    else
                    {
                        fileReader.ReadLine();
                    }
                }

                command = connection.CreateCommand();
                command.CommandText = "UPDATE Location SET istop = 0";
                command.ExecuteNonQuery();

                int locid = -1;
                while (true)
                {
                    command = connection.CreateCommand();
                    command.CommandText = String.Format("SELECT id, parent, ispoi, onquicklist, labelonhybrid, istop FROM Location WHERE id > {0}", locid);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        locid = reader.GetInt32(0);
                        Console.WriteLine(locid);
                        if (!reader.GetBoolean(5) && (reader.IsDBNull(1) || reader.GetBoolean(2) || reader.GetBoolean(3) || !reader.IsDBNull(4)))
                        {
                            int? parent = reader.IsDBNull(1) ? null : (int?)reader.GetInt32(0);
                            reader.Close();

                            command = connection.CreateCommand();
                            command.CommandText = String.Format("UPDATE Location SET istop = 1 WHERE id = {0}", locid);
                            command.ExecuteNonQuery();

                            bool istop = false;
                            while (parent != null && !istop)
                            {
                                command = connection.CreateCommand();
                                command.CommandText = String.Format("SELECT parent, istop FROM Location WHERE Id = {0}", parent);
                                reader = command.ExecuteReader();
                                reader.Read();
                                int? newparent = reader.IsDBNull(0) ? null : (int?)reader.GetInt32(0);
                                istop = reader.GetBoolean(1);
                                reader.Close();

                                if (!istop)
                                {
                                    command = connection.CreateCommand();
                                    command.CommandText = String.Format("UPDATE Location SET istop = 1 WHERE id = {0}", parent);
                                    command.ExecuteNonQuery();
                                }

                                parent = newparent;
                            }
                        }
                        else
                            reader.Close();
                    }
                    else break;
                }
            }

            Console.WriteLine("COMPLETED");
            Console.ReadKey();
        }
    }
}
