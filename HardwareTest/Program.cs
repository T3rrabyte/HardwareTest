using System;
using System.Collections.Generic;
using System.IO;
using System.Management;

namespace HardwareTest {
	internal class Program {
		private static int Main(string[] args) {
			Console.WriteLine("Performing a hardware test.");

			ManagementObjectSearcher s = new ManagementObjectSearcher("select * from CIM_LogicalDevice");

			// Get column headings.
			List<string> headings = new List<string>();
			foreach (ManagementObject o in s.Get()) {
				foreach (PropertyData p in o.Properties) {
					if (p.Value == null || ("" + p.Value).Trim() == "") { continue; }
					if (!headings.Contains(p.Name)) { headings.Add(p.Name); }
				}
			}

			Console.WriteLine("Found " + headings.Count + " unique properties in " + s.Get().Count + " management objects.");

			// Fill rows.
			List<List<string>> data = new List<List<string>> { headings };
			foreach (ManagementObject o in s.Get()) {
				List<string> row = new List<string>();
				foreach (string h in headings) {
					bool found = false;
					foreach (PropertyData p in o.Properties) {
						if (p.Name == h) {
							row.Add("" + p.Value);
							found = true;
							break;
						}
					}
					if (!found) { row.Add(""); }
				}
				data.Add(row);
			}

			string path = Environment.GetEnvironmentVariable("USERPROFILE") + "/.t3/HardwareTest/";
			Directory.CreateDirectory(path);
			Console.WriteLine("Exporting data to " + path);

			// Export data to CSV file.
			using (StreamWriter file = new StreamWriter(path + "hardware.csv")) {
				foreach (List<string> row in data) {
					foreach (string point in row) { file.Write("\"" + point + "\","); }
					file.WriteLine();
				}
			}

			return 0;
		}
	}
}
