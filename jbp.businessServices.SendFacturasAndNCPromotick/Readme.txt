Desarrollado por: Juan Francisco Esp�n
Fecha primer Relase: 05 jun 2019
-------------------------------------------------
Informaci�n del servicio:
 Este servicio tiene como funcionalidad hacer el envio autom�tico de facturas de participantes del plan
 puntos al sistema PROMOTICK

 Se puede parametrizar desde el archivo de configuraci�n, la frecuencia de ejecuci�n del servicio
 por ejemplo cada 60 seg. o en su defecto, que se ejecute todos los d�as a una hora espes�fica.

 Las facturas pueden enviarse por FTP o mediante el consumo de un servicio WEB

Para Instalar el servicio:
 Si est� instalado Visual Studio
  - Buscar el programa "Developer Command Pront" y ejecutar como administrador
  - ubicar desde el command pront el archivo jbp.businessServices.SendFacturasAndNCPromotick.exe (dentro de debug/bin)
  - ejecutar InstallUtil.exe jbp.businessServices.SendFacturasAndNCPromotick.exe
  - verificar que se haya instalado el servicio 
    - ventana + r
	- services.msc + enter

 Para Desinstalar el servicio:
  - Buscar el programa "Developer Command Pront" y ejecutar como administrador
  - ubicar desde el command pront el archivo jbp.businessServices.SendFacturasAndNCPromotick.exe (dentro de debug/bin)
  - ejecutar InstallUtil /u jbp.businessServices.SendFacturasAndNCPromotick.exe
  - verificar que se haya instalado el servicio 
    - ventana + r
	- services.msc + enter
 
 Para forzar la desinstalacion del servicio:
  - ejecutar como administrador "cmd"
  - sc delete "<serviceName>"
  - sc delete "JBP Envio de facturas y Notas de Credito a Promotick"
