<?php

namespace endpoint\version;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\database\NumericHelper;
use framework\entities\versions\VersionsService;
use stdClass;

class CheckVersion extends BaseEndpointBuilder
{
    private VersionsService $_versionService;
    public function __construct()
    {
        parent::__construct();
        $this->_versionService = new VersionsService();
    }
    public function defaultParams()
    {
        return [
            'installed' => "1.0.0",
            'cookie' => [],
        ];
    }
    public function build()
    {   
        $verArrStr = explode(".",$this->getParam('installed'));
        if(count($verArrStr) != 3){
            http_response_code(400);
            die();
        }
        $verArr = [];
        foreach($verArrStr as $verStr){
            $v = NumericHelper::toIntOrNull($verStr);
            if($v === null){
                http_response_code(400);
                die();
            }
            $verArr[] = $v;
        }
        $minVer = $this->_versionService->getItemsFromDB(['isminver' => [1]])[0];

        return $minVer->version <= $verArr[0] && $minVer->major <= $verArr[1] && $minVer->minor <= $verArr[2];
    }
}

