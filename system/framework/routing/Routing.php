<?php

namespace framework\routing;

use render\defaultBuild\Header;
use render\defaultBuild\Footer;
use framework\entities\tokens\TokensService;

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
                $externalParams = [];

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

                $externalParams['cookie'] = $cookie;

                $header = new Header();
                $footer = new Footer();
                $className = self::RENDER_NAMESPACE . $class['className'];
                $classObject = new $className();

                $externalParams = array_merge($externalParams, $header->getExternalParams());
                $externalParams = array_merge($externalParams, $classObject->getExternalParams());
                $externalParams = array_merge($externalParams, $footer->getExternalParams());

                $header->render($externalParams);
                $classObject->render($externalParams);
                $footer->render($externalParams);
            } elseif ($requestType === "POST") {
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
