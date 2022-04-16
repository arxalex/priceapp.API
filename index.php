<?php

use framework\routing\Routing;

const FOLDER_NAME = "system";

$uri_string = parse_url($_SERVER['REQUEST_URI'])['path'];
$uri = explode("/", $uri_string);

require_once("SplClassLoader.php");

$namespaces = [
    //'businessLogic',
    'businessLogic\items',
    'businessLogic\prices',
    //'endpoint',
    'endpoint\brands',
    'endpoint\categories',
    'endpoint\consists',
    'endpoint\countries',
    'endpoint\defaultBuild',
    'endpoint\filials',
    'endpoint\items',
    'endpoint\login',
    'endpoint\packages',
    'endpoint\prices',
    //'framework',
    //'framework\core',
    'framework\core\constants',
    'framework\database',
    //'framework\entities',
    'framework\entities\brands',
    'framework\entities\categories',
    'framework\entities\categories_link',
    'framework\entities\consists',
    'framework\entities\countries',
    'framework\entities\default_entities',
    'framework\entities\filials',
    'framework\entities\ipaddresses',
    'framework\entities\items',
    'framework\entities\items_link',
    'framework\entities\packages',
    'framework\entities\prices',
    'framework\entities\prices_history',
    'framework\entities\shops',
    'framework\entities\tokens',
    'framework\entities\users',
    'framework\routing',
    //'framework\shops',
    'framework\shops\atb',
    'framework\shops\silpo',
    'framework\shops\common',
    'framework\utils',
    //'render',
    'render\admin',
    'render\defaultBuild',
    'render\login',
    'render\main',
    'viewModels'
];

foreach($namespaces as $value){
    $loader = new SplClassLoader($value, FOLDER_NAME);
    $loader->register();
}

Routing::resolveRoute($uri_string, $_SERVER['REQUEST_METHOD']);




