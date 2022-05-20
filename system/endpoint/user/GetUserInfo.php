<?php

namespace endpoint\user;

use endpoint\defaultBuild\BaseEndpointBuilder;
use stdClass;

class GetUserInfo extends BaseEndpointBuilder
{
    public function __construct()
    {
        parent::__construct();
    }
    public function defaultParams()
    {
        return [
            'cookie' => [],
        ];
    }
    public function build()
    {        
        $this->_usersService->unavaliableRequest($this->getParam('cookie'), 1);

        return $this->_usersService->getUserInfoByCookie($this->getParam('cookie'));
    }
}

