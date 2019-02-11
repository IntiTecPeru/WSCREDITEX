using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WSCREDITEX
{
    class Program
    {
        //URL base para el consumo de los WS
        static String  urlMain = "http://5.189.151.80:8077/api/";
        
        static void Main(string[] args)
        {   
            //Se obtiene el token para  poder consumir los demas WS.
           String token = getToken("CBenites@creditex.com.pe", "12345", "BD_CREDITEX");
            if (token != null) {
                //Obtenermos los clientes 
               dynamic clients = getClients(token,"o_update");
                //Actualizamos los clientes recibidos
                updateClients(token, clients);
            }
        }
        //Metodo para Actualizar Clientes en Odoo
        public static void updateClients(String token,dynamic clients) {
            dynamic json_cients_updt = null;
            String url = urlMain + "res.update_tim";
            var webrequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
            if (webrequest != null)
            {
                //Se debe construir los datos en JSON
                String  data_clients= "[";
                String item_clients = "";
                foreach (dynamic item in clients.data) {
                    item_clients = item_clients+ "{'id_partner':" + item .id+ ",'it_id_tim':7},";
                }
                data_clients = data_clients + item_clients + "]";

                var postData = "partner_ids=" + data_clients;
                var data = Encoding.ASCII.GetBytes(postData);
                webrequest.Method = "POST";
                webrequest.ContentType = "application/x-www-form-urlencoded";
                //Se tiene que enviar el Token que hemos obtenido
                webrequest.Headers.Add("access_token", token);
                webrequest.ContentLength = data.Length;
                using (var stream = webrequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)webrequest.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //Lista de los Clientes.
                json_cients_updt = JsonConvert.DeserializeObject(responseString);
                Console.WriteLine(responseString);
                Console.ReadLine();
            }

        }
        //Consumir los WS de Clientes
        public static dynamic getClients(String token,String status) {
            dynamic json_cients = null;
            String url = urlMain + "res.partner_search";
            var webrequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
            if (webrequest != null)
            {
                //De este modo enviamos los parametros en el formato application/x-www-form-urlencoded
                var postData = "it_state="+status;
                var data = Encoding.ASCII.GetBytes(postData);
                // Todas las consultas estaran en este metodo.
                webrequest.Method = "POST";
                webrequest.ContentType = "application/x-www-form-urlencoded";
                //Se tiene que enviar el Token que hemos obtenido
                webrequest.Headers.Add("access_token", token);
                webrequest.ContentLength = data.Length;
                using (var stream = webrequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)webrequest.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //Lista de los Clientes.
                json_cients = JsonConvert.DeserializeObject(responseString);
                //Console.WriteLine(responseString);
                //Console.ReadLine();
            }
            return json_cients;
        }
         //Metodo para obtener el WS
        public static String getToken(String login,String password,String bd) {
            String tokenMain = null;
            //Se envian los parametros del token.
            String url = urlMain+ "auth/token?login="+ login + "&password="+ password + "&db="+bd+"";
            var webrequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
            if (webrequest != null) {
                webrequest.Method = "POST";
                webrequest.Timeout = 12000;
                using (var response = webrequest.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var result = reader.ReadToEnd();
                    String resultString = Convert.ToString(result);
                    // Se realiza la conversion del objeto JSON a Dynamiv
                    dynamic json = JsonConvert.DeserializeObject(resultString);
                    //Console.WriteLine(json.access_token);
                    tokenMain = json.access_token;

                }
            }
            return tokenMain;



        }
    }
}
