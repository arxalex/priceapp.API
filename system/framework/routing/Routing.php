<?php

namespace framework\routing;

use render\defaultBuild\Header;
use render\defaultBuild\Footer;
use endpoint\defaultBuild\BaseEndpointBuilder;

class Routing
{
    private const RENDER_NAMESPACE = "render\\";
    private const ENDPOINT_NAMESPACE = "endpoint\\";

    public static function resolveRoute(string $uri_string, $requestType)
    {
        $routeRegistrer = new RouteRegister();
        $class = $routeRegistrer->getClassByRoute($uri_string);

        if ($class == null) {
            http_response_code(404);
            exit;
        } else {
            if ($requestType === "GET") {
                $header = new Header();
                $footer = new Footer();
                $className = self::RENDER_NAMESPACE . $class['className'];
                $classObject = new $className();

                $externalParams = [];

                $externalParams = array_merge($externalParams, $header->getExternalParams());
                $externalParams = array_merge($externalParams, $classObject->getExternalParams());
                $externalParams = array_merge($externalParams, $footer->getExternalParams());

                $header->render($externalParams);
                $classObject->render($externalParams);
                $footer->render($externalParams);
            } elseif ($requestType === "POST") {
                $className = self::ENDPOINT_NAMESPACE . $class['className'];
                $classObject = new $className();
                $classObject->render(json_decode(file_get_contents("php://input"), true));
            } else {
                http_response_code(404);
            }
        }
    }
}
