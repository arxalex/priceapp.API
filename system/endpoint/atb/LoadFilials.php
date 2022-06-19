<?php

namespace endpoint\atb;

use atb\AtbFilials;
use endpoint\defaultBuild\BaseEndpointBuilder;

class LoadFilials extends BaseEndpointBuilder
{
    private AtbFilials $_atbFilials;
    public function __construct()
    {
        parent::__construct(); 
        $this->_atbFilials = new AtbFilials();
    }
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));
        return $this->_atbFilials->loadFilials();
    }
}
