<?php

use framework\routing\Routing;

set_time_limit(0);

const FOLDER_NAME = "system";

$uri_string = parse_url($_SERVER['REQUEST_URI'])['path'];
$uri = explode("/", $uri_string);

require_once("SplClassLoader.php");

$namespaces = [
    'atb',
    //'endpoint',
    'endpoint\atb',
    'endpoint\defaultBuild',
    'endpoint\login',
    //'framework',
    //'framework\core',
    'framework\core\constants',
    'framework\database',
    'framework\routing',
    //'framework\entities',
    'framework\entities\categories',
    'framework\entities\constants',
    'framework\entities\default_entities',
    'framework\entities\filials',
    'framework\entities\items',
    'framework\entities\prices',
    'framework\entities\tokens',
    'framework\entities\users',
    'framework\utils'
];

foreach($namespaces as $value){
    $loader = new SplClassLoader($value, FOLDER_NAME);
    $loader->register();
}

Routing::resolveRoute($uri_string, $_SERVER['REQUEST_METHOD']);




