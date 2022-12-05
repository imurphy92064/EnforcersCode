using System;
using System.Diagnostics;
using System.Text;

public class Sensitivity
{
    public static float getSensitivity()
    {
        bool didReadFromFile = false;
        float sensToReturn = 4.50f;
        try
        {
            if (UnityEngine.Windows.File.Exists("Sensitivity.txt"))
            {
                //ASCII Bytes -> String -> Float
                byte[] rBytes = UnityEngine.Windows.File.ReadAllBytes("Sensitivity.txt");
                string rString = Encoding.ASCII.GetString(rBytes);
                sensToReturn = Convert.ToSingle(rString);
                didReadFromFile = true;
            }
        }
        catch (Exception){ }

        if (!didReadFromFile)
        {
            try
            {
                //Float -> String -> ASCII Bytes
                string wFloat = sensToReturn.ToString();
                byte[] wBytes = Encoding.ASCII.GetBytes(wFloat);
                UnityEngine.Debug.Log("Supposedly wrote to sens1");
                UnityEngine.Windows.File.WriteAllBytes("Sensitivity.txt",wBytes);
                UnityEngine.Debug.Log("Supposedly wrote to sens2");
            }
            catch(Exception e){ UnityEngine.Debug.Log(e.Message); }
        }

        return sensToReturn;
    }
}
