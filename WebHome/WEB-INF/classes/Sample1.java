import java.io.PrintWriter;
import java.io.IOException;
import java.io.*;
import java.util.*;
import javax.servlet.*;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;


public class Sample1 extends HttpServlet
{
	public void doGet(HttpServletRequest request,HttpServletResponse response)
						throws ServletException,IOException
	{
		response.setContentType("text/html;charset=UTF-8");
		PrintWriter out = response.getWriter();
		
		out.println("<!DOCTYPE html><html>");
		//out.println("<head><meta charset='UTF-8' /><title>これは、MockServerのサンプルです</title></head>");
		out.println("<head><meta charset='UTF-8' /><title>Sample1(Get)</title></head>");
		out.println("<body>");
		out.println("<p>Hello World by Get</p><br><br>");
		
		
		out.println("<p>Header</p>");
		out.println("<table>");
		out.println("<tr><th>name</th><th>value</th></tr>");
		Enumeration<String> headers = request.getHeaderNames();
		while(headers.hasMoreElements()){
			String name = (String)headers.nextElement();
			String value = request.getHeader(name);
			out.println("<tr><td>" + name + "</td><td>" + value + "</td></tr>");
		}
		out.println("</table>");
		
		
		out.println("<p>Parameter</p>");
		out.println("<table>");
		out.println("<tr><th>name</th><th>value</th></tr>");
		Enumeration<String> params = request.getParameterNames();
		while(params.hasMoreElements()){
			String pname = (String)params.nextElement();
			String[] pvalues = request.getParameterValues(pname);
			out.print("<tr><td>" + pname + "</td><td>");
			for(int i = 0;i < pvalues.length;i++){
				if(i == 0)out.print(",");
				out.print(pvalues[i]);
			}
			out.println("</td></tr>");
		}
		out.println("</table>");
	
		out.println("</body>");
		out.println("</html>");
		
	}
	public void doPost(HttpServletRequest request,HttpServletResponse response)
						throws ServletException,IOException
	{
		request.setCharacterEncoding("UTF-8");
		response.setContentType("text/html;charset=UTF-8");
		PrintWriter out = response.getWriter();
		
		ServletContext application=this.getServletContext();
		String path=application.getRealPath("/WEB-INF/data/memo.txt");
		FileOutputStream fos = new FileOutputStream(path,true);
		OutputStreamWriter osw=new OutputStreamWriter(fos,"UTF-8");
		BufferedWriter fout =new BufferedWriter(osw);
		
		String tmpst;
		
		tmpst = "*******Start";
		fout.write(tmpst,0,tmpst.length());fout.newLine();
		
		tmpst = "QueryString = ";
		fout.write(tmpst,0,tmpst.length());
		tmpst = request.getQueryString();
		if(tmpst == null) tmpst = "null";
		fout.write(tmpst,0,tmpst.length());fout.newLine();
		
		tmpst = "RequestURI = ";
		fout.write(tmpst,0,tmpst.length());
		tmpst = request.getRequestURI();
		fout.write(tmpst,0,tmpst.length());fout.newLine();
		
		tmpst = "RequestURL = ";
		fout.write(tmpst,0,tmpst.length());
		tmpst = request.getRequestURL().toString();;
		fout.write(tmpst,0,tmpst.length());fout.newLine();
		
		out.println("<!DOCTYPE html><html>");
		//out.println("<head><meta charset='UTF-8' /><title>これは、MockServerのサンプルです</title></head>");
		out.println("<head><meta charset='UTF-8' /><title>Sample1(Post)</title></head>");
		out.println("<body>");
		out.println("<p>Hello World by Post</p><br><br>");
		
		
		out.println("<p>Header</p>");
		out.println("<table>");
		out.println("<tr><th>name</th><th>value</th></tr>");
		Enumeration<String> headers = request.getHeaderNames();
		while(headers.hasMoreElements()){
			String name = (String)headers.nextElement();
			String value = request.getHeader(name);
			out.println("<tr><td>" + name + "</td><td>" + value + "</td></tr>");
		}
		out.println("</table>");
		
		
		out.println("<p>Parameter</p>");
		out.println("<table>");
		out.println("<tr><th>name</th><th>value</th></tr>");
		Enumeration<String> params = request.getParameterNames();
		while(params.hasMoreElements()){
			String pname = (String)params.nextElement();
			String[] pvalues = request.getParameterValues(pname);
			out.print("<tr><td>" + pname + "</td><td>");
			for(int i = 0;i < pvalues.length;i++){
				if(i == 0)out.print(",");
				out.print(pvalues[i]);
			}
			out.println("</td></tr>");
		}
		out.println("</table>");
	
		out.println("<p>Content</p>");
		out.print("<pre>");
		BufferedReader in = request.getReader();
		String st;
		while((st = in.readLine()) != null){
			out.print(st);
			fout.write(st,0,st.length());
		}
		out.print("</pre>");
		
		out.println("</body>");
		out.println("</html>");
		
		fout.newLine();
		fout.close();
	}
}
