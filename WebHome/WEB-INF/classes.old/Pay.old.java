import java.io.PrintWriter;
import java.io.IOException;
import java.io.*;
import java.util.*;
import javax.servlet.*;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;


public class Pay extends HttpServlet
{
	public void doGet(HttpServletRequest request,HttpServletResponse response)
						throws ServletException,IOException
	{
		doPost(request,response);		
	}
	public void doPost(HttpServletRequest request,HttpServletResponse response)
						throws ServletException,IOException
	{
		ServletContext application=this.getServletContext();
		String path=application.getRealPath("/WEB-INF/log/payLog.txt");
		OutputStream log_stream = new FileOutputStream(path,true);
		PrintWriter log = new PrintWriter(log_stream);
		
		String tmpst;
		
		log.println("*******Start*******");
		
		tmpst = request.getQueryString();
		if(tmpst == null) tmpst = "null";
		log.println("QueryString:" + tmpst);
		
		tmpst = request.getRequestURI();
		log.println("RequestURI:" + tmpst);
		
		tmpst = request.getRequestURL().toString();
		log.println("RequestURL:" + tmpst);
				
		log.println("#Header");
		Enumeration<String> headers = request.getHeaderNames();
		while(headers.hasMoreElements()){
			String name = (String)headers.nextElement();
			String value = request.getHeader(name);
			log.println(name + ":" + value);
		}
		log.println("#");
		
		log.println("#Parameter");
		Enumeration<String> params = request.getParameterNames();
		while(params.hasMoreElements()){
			String pname = (String)params.nextElement();
			String[] pvalues = request.getParameterValues(pname);
			log.print(pname + ":");
			for(int i = 0;i < pvalues.length;i++){
				if(i != 0)log.print(",");
				log.print(pvalues[i]);
			}
			log.println();
		}
		log.println("#");
	
		log.println("#Content");
		BufferedReader in = request.getReader();
		String st;
		while((st = in.readLine()) != null){
			log.println(st);
		}
		log.print("#");
		
		log.println();
		log.flush();
		log.close();
		
		path=application.getRealPath("/WEB-INF/data/payData.txt");
		FileInputStream file = new FileInputStream(path);
		
		request.setCharacterEncoding("UTF-8");
		//response.setContentType("text/html;charset=UTF-8");
		OutputStream out = response.getOutputStream();
		
		byte[] data = new byte[1024];
		int n;
		while((n = file.read(data)) > 0){
			out.write(data,0,n);
		}
		
		file.close();
		
		out.flush();
		out.close();
	}
}
