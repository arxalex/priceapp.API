<?php

namespace endpoint\login;

use endpoint\defaultBuild\BaseEndpointBuilder;
use stdClass;

class Logout extends BaseEndpointBuilder
{
    public function __construct()
    {
        parent::__construct();
    }
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'), 1);
        $result = new stdClass();
        $result->statusLogout = $this->_usersService->logoutUser($this->getParam('cookie'));

        return $result;
    }
}
