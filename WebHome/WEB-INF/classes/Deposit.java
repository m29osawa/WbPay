import java.io.PrintWriter;
import java.io.IOException;
import java.io.*;
import java.util.*;
import javax.servlet.*;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;


public class Deposit extends HttpServlet
{
	public void doGet(HttpServletRequest request,HttpServletResponse response)
						throws ServletException,IOException
	{
		ServletContext application=this.getServletContext();
		ServerUtil.printRequestLog(application,request,"DepositLog.txt");	
		ServerUtil.responseData(application,response,"DepositData.txt");

	}
	public void doPost(HttpServletRequest request,HttpServletResponse response)
						throws ServletException,IOException
	{
	
		ServletContext application=this.getServletContext();
		ServerUtil.printRequestLog(application,request,"DepositLog.txt");	
		ServerUtil.responseData(application,response,"DepositData.txt");
	}
}
