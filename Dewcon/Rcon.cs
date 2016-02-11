﻿using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Dewcon
{
  static class Rcon
  {
    public static string DewCmd(string cmd, string ip = "127.0.0.1", int port = 2448)
    {
      TcpClient client;
      try
      {
        client = new TcpClient(ip, port);
      }
      catch (SocketException)
      {
        return Environment.NewLine + "ElDorito is not running. You can start eldorito with 'start <ElDorito path>' or by starting ElDorito normally.";
      }
      byte[] data = Encoding.GetEncoding(1252).GetBytes(cmd);
      MemoryStream memStream = new MemoryStream();
      NetworkStream stm = client.GetStream();
      stm.Write(data, 0, data.Length);
      byte[] resp = new byte[2048];
      
      int bytes;
      client.Client.ReceiveTimeout = 20;
      do
      {
        try
        {
          bytes = stm.Read(resp, 0, resp.Length);
          memStream.Write(resp, 0, bytes);
        }
        catch (IOException ex)
        {
          SocketException socketExept = ex.InnerException as SocketException;
          if (socketExept == null || socketExept.ErrorCode != 10060)
            throw;
          bytes = 0;
        }
      } while (bytes > 0);
      var lines = Encoding.GetEncoding(1252).GetString(memStream.ToArray()).Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray();
      var output = string.Join(Environment.NewLine, lines);
      client.Close();
      memStream.Close();
      stm.Close();
      return Regex.Replace(output, Environment.NewLine + Environment.NewLine, Environment.NewLine, RegexOptions.Multiline);
    }
  }

}
