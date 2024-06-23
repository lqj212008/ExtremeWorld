using System.IO;
using UnityEngine;
using UnityEditor;

public class CodeTool {

	static string calcPath = "Assets/Scripts";
	[MenuItem("Tool/统计代码行数")]
	static void CalcCode()
	{
		if (!Directory.Exists(calcPath))
		{
			Debug.LogErrorFormat("Path Not Exist : \"{0}\" ", calcPath);
			return;
		}
		string[] fileName = Directory.GetFiles(calcPath, "*.cs", SearchOption.AllDirectories);
		int totalLine = 0;
		foreach(var temp in fileName)
		{
			int nowLine = 0;
			StreamReader sr = new StreamReader(temp);
			while(sr.ReadLine() != null)
			{
				nowLine++;
			}

			totalLine += nowLine;
		}

		Debug.LogFormat("代码总行数: {0} -> 代码总文件数: {1}", totalLine, fileName.Length);
	}
}
