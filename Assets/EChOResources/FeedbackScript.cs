using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class FeedbackScript : MonoBehaviour {

	string address;
	string name; 
	string message;

	public void setName(string yourName){

		name = yourName;
		Debug.Log (name);
	}

	public void setMessage(string yourMessage){
	
		message = yourMessage;
		Debug.Log (message);
	}

	public void Send_Email(){
		
		Debug.Log (name);
		Debug.Log (message);

		MailMessage email = new MailMessage ();

		email.From = new MailAddress ("echouserfeedback@gmail.com");
		email.To.Add ("buildfeedbacksugarpoke@gmail.com");
		email.Subject = "Feedback from " + name;
		email.Body = message;

		SmtpClient smtpServer = new SmtpClient ("smtp.gmail.com");
		smtpServer.Port = 587;
		smtpServer.Credentials = new System.Net.NetworkCredential ("echouserfeedback@gmail.com", "EChOPassword") as ICredentialsByHost;
		smtpServer.EnableSsl = true;
		ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, 
		                                                                   SslPolicyErrors sslPolicyErrors) {
			return true;
		};

		smtpServer.Send(email);
		Debug.Log ("Success!");




	}


}
