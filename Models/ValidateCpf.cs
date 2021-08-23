using CoreBot.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ValidacoesLibrary
{
    
    public class Validacoes

    {
        public bool ValidarCep(String cep)
        {
            bool cepValido = false;

            string emailRegex = string.Format("^d{ 5}-d{ 3}$");

            try
            {
                cepValido = Regex.IsMatch(cep, emailRegex);
            }
            catch (RegexMatchTimeoutException)
            {
                cepValido = false;
            }

            return cepValido;
        }

        public bool ValidarPhone(string phone)
        {
            Regex regex = new Regex(@"^[1-9]{2}9[7-9]{1}[0-9]{3}[0-9]{4}$");

            if (regex.IsMatch(phone))
            {
                return true; //se a placa for válida, retorna TRUE
            }
            else
            {
                return false; //se a placa for inválida, retorna FALSE    
            }
                
                
        }

        public bool ValidarEmail(String email)
        {
            bool emailValido = false;

            string emailRegex = string.Format("{0}{1}",
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))",
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");

            try
            {
                emailValido = Regex.IsMatch(email,emailRegex);
            }
            catch (RegexMatchTimeoutException)
            {
                emailValido = false;
            }

            return emailValido;
        }



        public bool validaPlaca(string value)
        {
            Regex regex = new Regex(@"^[a-zA-Z]{3}\-\d{4}$");

            if (regex.IsMatch(value))
                return true; //se a placa for válida, retorna TRUE
                return false; //se a placa for inválida, retorna FALSE             
        }




        public static bool ValidaCPF(string vrCPF)

        {
            var renovatiofields = new RenovationFields();

            string valor = vrCPF.Replace(".", "");

            valor = valor.Replace("-", "");

            if (renovatiofields.IsNumeric(valor) == false)
            {
                return false;
            }

            if (valor.Length != 11)

                return false;

            bool igual = true;

            for (int i = 1; i < 11 && igual; i++)

                if (valor[i] != valor[0])

                    igual = false;

            if (igual || valor == "12345678909")

                return false;


            int[] numeros = new int[11];


            for (int i = 0; i < 11; i++)

                numeros[i] = int.Parse(

                  valor[i].ToString());


            int soma = 0;

            for (int i = 0; i < 9; i++)

                soma += (10 - i) * numeros[i];


            int resultado = soma % 11;


            if (resultado == 1 || resultado == 0)

            {

                if (numeros[9] != 0)

                    return false;

            }

            else if (numeros[9] != 11 - resultado)

                return false;



            soma = 0;

            for (int i = 0; i < 10; i++)

                soma += (11 - i) * numeros[i];



            resultado = soma % 11;



            if (resultado == 1 || resultado == 0)

            {

                if (numeros[10] != 0)

                    return false;
            }

            else

                if (numeros[10] != 11 - resultado)

                return false;



            return true;

        }

    }

}