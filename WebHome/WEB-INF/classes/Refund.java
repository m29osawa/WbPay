import java.io.PrintWriter;
import java.io.IOException;
import java.io.*;
import java.util.*;
import javax.servlet.*;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;


public class Refund extends HttpServlet
{
	public void doGet(HttpServletRequest request,HttpServletResponse response)
						throws ServletException,IOException
	{
		ServletContext application=this.getServletContext();
		ServerUtil.printRequestLog(application,request,"RefundLog.txt");	
		ServerUtil.responseData(application,response,"RefundData.txt");

	}
	public void doPost(HttpServletRequest request,HttpServletResponse response)
						throws ServletException,IOException
	{
	
		ServletContext application=this.getServletContext();
		ServerUtil.printRequestLog(application,request,"RefundLog.txt");	
		ServerUtil.responseData(application,response,"RefundData.txt");
	}
}
