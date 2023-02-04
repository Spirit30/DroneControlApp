#if UNITY_EDITOR
using Renci.SshNet;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace SSHModule
{
    public class SSHTest : MonoBehaviour
    {
        IEnumerator Start()
        {
            using (var client = new SshClient("192.168.100.206", "admin", "blackbay"))
            {
                client.Connect();

                var command = client.CreateCommand("python3");
                command.BeginExecute();

                while(enabled)
                {
                    yield return new WaitForEndOfFrame();
                    byte[] bytes = new byte[2048];
                    command.OutputStream.Read(bytes, 0, bytes.Length);
                    string output = Encoding.UTF8.GetString(bytes);
                    Debug.Log("O: " + output);
                }

                client.Disconnect();
            }
        }

        //IEnumerator Start()
        //{
        //    using (var client = new SshClient("", "", ""))
        //    {
        //        client.Connect();

        //        client.RunCommand("gpio mode 24 out");
        //        client.RunCommand("gpio write 24 1");

        //        yield return new WaitForSeconds(3);

        //        client.RunCommand("gpio write 24 0");
        //        client.RunCommand("gpio mode 24 in");

        //        client.Disconnect();
        //    }
        //}

        //void Start()
        //{
        //    using (var client = new SshClient("", "", ""))
        //    {
        //        client.Connect();

        //        var command = client.RunCommand("gpio readall");
        //        Debug.Log(command.Result);

        //        client.Disconnect();
        //    }
        //}

        //void Start()
        //{
        //    using (var client = new SshClient("", "", ""))
        //    {
        //        client.Connect();

        //        Debug.Log("client.IsConnected: " + client.IsConnected);

        //        var output = client.RunCommand("hostname -I");

        //        Debug.Log(output.CommandText);
        //        Debug.Log(output.Result);

        //        client.Disconnect();
        //    }
        //}
    }
}
#endif