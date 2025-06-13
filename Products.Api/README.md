API Client
----------
Recently reviewed Scalar, an open-source Scalar is an open-source API platform. Integrated it as an alternative to Swagger.
https://scalar.com
https://guides.scalar.com/scalar/introduction


Authorisation
-------------
Implemented the OAuth Client Credentials flow using Azure AD on the ProductsController.

Implemented an AuthorisationController to workaround a CORS error when retieving a token.


Health Endpoint
---------------
Available at the endpoint /_health
Enabled the built in implemetation rather than creating a dedicated Controller. Relevant methods are builder.Services.AddHealthChecks() and app.MapHealthChecks().

This may be extended by implementing a class that inherits the IHealthCheck interface and then registering it via builder.Services.AddHealthChecks(). This is useful for checking the health status of other components of the API, such as databases or even external microservices.


Exception Handling
------------------
Implemented an Exception handler ExceptionToProblemDetailsHandler and registered it via builder.Services.AddExceptionHandler(). The handler converts Exceptions to ProblemDetails and enriches the information returned. Also log all exceptions here.


Colour Enum
-----------
Implemented Product colours as an Enum. Advantage here is strong typing. However, there is a lack of extensibility as Enums have a fixed set of values defined at compile-time, making it challenging to add or modify values dynamically at runtime. If dynamic modification is a requirement, then an alternative design, such as persisting colours in a database table and providing functionality to update them would be a better approach.