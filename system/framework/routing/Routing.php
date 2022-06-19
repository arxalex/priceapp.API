<?php

namespace framework\routing;

use framework\entities\tokens\TokensService;

class Routing
{
    private const ENDPOINT_NAMESPACE = "endpoint\\";

    public static function resolveRoute(string $uri_string, $requestType)
    {
        $routeRegistrer = new RouteRegister();

        if ($requestType === "GET"){
            echo '<!DOCTYPE html>
            <html>
                <head>
                    <meta http-equiv="Content-type" content="text/html; charset=utf-8">
                    <meta http-equiv="Cache-control" content="no-cache">
                    <meta http-equiv="Pragma" content="no-cache">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>proxy</title>
                </head>
                <body>
                    <h1>Nive, you got access to proxy subdomain</h1>
                </body>
            </html>';
            die();
        }

        $class = $routeRegistrer->getClassByRoute($uri_string);

        if ($class == null) {
            http_response_code(404);
            exit;
        } else {
            if ($requestType === "POST") {
                $className = self::ENDPOINT_NAMESPACE . $class['className'];
                $classObject = new $className();

                $params = json_decode(file_get_contents("php://input"), true);
                $cookie = [];
                if(isset($_COOKIE["userid"]) 
                    && isset($_COOKIE["token"]) 
                    && isset($_COOKIE["token_expires"])){
                    $tokenService = new TokensService();
                    $userid = $_COOKIE["userid"];
                    $token = $_COOKIE["token"];
                    $expires = $_COOKIE["token_expires"];

                    $isValid = $tokenService->isTokenValid($userid, $token, $expires);

                    if($isValid) {
                        if($expires - 86400 < time()){
                            $newToken = $tokenService->reValidToken($userid, $token, $expires);
                            $token = $newToken->token;
                            $expires = $newToken->expires;

                            setcookie("userid", $userid, time() + 2592000, '/');
                            setcookie("token", $token, $expires, '/');
                            setcookie("token_expires", $expires, $expires, '/');
                        }
                        $cookie['userid'] = $userid;
                        $cookie['token'] = $token;
                        $cookie['token_expires'] = $expires;
                    } else {
                        http_response_code(403);
                    }
                }

                $params['cookie'] = $cookie;

                $classObject->render($params);
            } else {
                http_response_code(404);
            }
        }
    }
}
