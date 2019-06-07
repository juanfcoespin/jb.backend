Para Instalar el servicio:
 Si está instalado Visual Studio
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
