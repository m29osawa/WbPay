import java.io.*;
import java.util.*;
import javax.servlet.*;
import javax.servlet.http.*;

public class ServerUtil extends HttpServlet
{
	static public void printRequestLog(ServletContext application,HttpServletRequest request,String fname)
						throws ServletException,IOException
	{
		request.setCharacterEncoding("UTF-8");
		
//		ServletContext application=this.getServletContext();
		String path=application.getRealPath("/WEB-INF/log/" + fname);
		OutputStream log_stream = new FileOutputStream(path,true);
		PrintWriter log = new PrintWriter(log_stream);
		
		String	tmpst;
		int		tmpint;
		
		log.println("*******REQUEST*******");
		
		tmpst = request.getRequestURL().toString();
		log.println("RequestURL:" + tmpst);
		
		tmpst = request.getRequestURI();
		log.println("RequestURI:" + tmpst);
				
		tmpst = request.getContextPath();
		log.println("ContextPath:" + tmpst);
		
		tmpst = request.getServletPath();
		log.println("ServletPath:" + tmpst);
		
		tmpst = request.getPathInfo();
		log.println("PathInfo:" + tmpst);
		
		tmpst = request.getQueryString();
		if(tmpst == null) tmpst = "null";
		log.println("QueryString:" + tmpst);
		
		tmpst = request.getMethod();
		log.println("Method:" + tmpst);
		
		tmpint = request.getContentLength();
		log.println("ContentLength:" + tmpint);
		
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
	}
	
	
	static public void responseData(ServletContext application,HttpServletResponse response,String fname)
						throws ServletException,IOException
	{
		String path=application.getRealPath("/WEB-INF/data/" + fname);
		FileInputStream file = new FileInputStream(path);
		
		response.setContentType("application/json;charset=UTF-8");
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
