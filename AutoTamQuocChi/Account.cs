using System;

public class Account
{
    private string username;
    private string password;


    public Account()
	{
	}

    public string Username { get => username; set => username = value; }
    public string Password { get => password; set => password = value; }
}
