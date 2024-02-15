# URL API PUBLICADA:
https://4667-2806-1016-6-f36e-2d22-14df-5eb2-6297.ngrok-free.app/Health

# Swagger API implementado:
https://4667-2806-1016-6-f36e-2d22-14df-5eb2-6297.ngrok-free.app/swagger/index.html

# Proyecto con Clean Architecture en ASP.NET 5
Aquí hemos implementado la arquitectura Clean Architecture junto con PostgreSQL como base de datos. Esta arquitectura nos permite crear sistemas altamente mantenibles, testables y escalables.

## ¿Qué es Clean Architecture?
Clean Architecture es un enfoque de diseño de software que se centra en la separación de preocupaciones y la independencia de frameworks externos. Se basa en los principios SOLID y promueve la creación de sistemas altamente mantenibles, testables y escalables.

## Ventajas de Clean Architecture
Independencia de Frameworks: Los detalles de la infraestructura, como las bases de datos o los frameworks de UI, están separados de la lógica de negocio, lo que facilita la adaptación a cambios tecnológicos.

Testabilidad: La separación de capas permite escribir pruebas unitarias y de integración de forma más sencilla, ya que la lógica de negocio no está acoplada a la infraestructura.

Mantenibilidad: La arquitectura limpia fomenta la modularidad y la cohesión, lo que facilita la identificación y el mantenimiento de diferentes componentes del sistema.

Escalabilidad: Al tener una estructura bien definida y desacoplada, es más fácil escalar y extender la aplicación a medida que los requisitos cambian o crecen.

## Coleccion exportado endpoints PostMan.
Dentro de la carpeta "Coleccion para postman" viene la coleccion, con las variables configuradas y los endpoints existentes.

## Requerimientos para Ejecutar la API
- Antes de ejecutar la API, asegúrate de tener PostgreSQL instalado y una base de datos vacía.
- Instalar el SDK de .NET 5: Asegúrate de tener instalado el SDK de .NET 5 en tu máquina. Puedes descargarlo desde el sitio web oficial de .NET.
- Los siguientes pasos tambien incluye utilizar Visual Studio, de preferencia tenerlo instalado.
## Pasos para Ejecutar la API:
Abrir el Proyecto: Abre la solución de tu proyecto ASP.NET Core 5 en tu entorno de desarrollo favorito, como Visual Studio.

### Configuración de la API:

Revisa y ajusta el archivo appsettings.Development.json para configurar las variables de conexión a la base de datos ### ConnectionStrings.APP.
<img width="868" alt="image" src="https://github.com/Riichhard97/prueba_tecnica/assets/62078290/70fad1d4-c6b2-4687-8dc9-148260dd3147">

Ejecución de Pruebas
Para ejecutar pruebas en Visual Studio, sigue estos pasos:

Seleccionar Pruebas a Ejecutar: Abre la ventana "Explorador de pruebas" en Visual Studio y selecciona las pruebas que deseas ejecutar.

Ejecutar Pruebas: Haz clic derecho sobre las pruebas seleccionadas y selecciona "Ejecutar seleccionado" o "Depurar seleccionado".

Ver Resultados de las Pruebas: Después de ejecutar las pruebas, puedes ver los resultados en la ventana "Resultados de pruebas".

## Pruebas Creadas
Se crearon 4 pruebas, validando algunas reglas de negocio del ejercicio:

- AddBrandOnlyWithName
- AddModelWithUnknownBrandThrowError
- AddModelWithLessThan100000InAveragePrice
- AddModelWithNameRepeatedInTheSameBrand

#### Actualización
Se agregó una colección de Postman para realizar las pruebas, la cual se encuentra en la carpeta coleccion para postman.
