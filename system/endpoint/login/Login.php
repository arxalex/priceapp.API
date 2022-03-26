<?php

namespace endpoint\login;

use endpoint\defaultBuild\BaseEndpointBuilder;
use stdClass;

class Login extends BaseEndpointBuilder
{
    public function __construct()
    {
        parent::__construct();
    }
    public function defaultParams()
    {
        return [
            'cookie' => [],
            'username' => '',
            'password' => ''
        ];
    }
    public function build()
    {
        $result = new stdClass();
        
        if($this->_usersService->isLoggedInUser($this->getParam('cookie'))){
            $result->statusLogin = false;
            return $result;
        }

        if($this->getParam('username') == "" || $this->getParam('password') == ""){
            http_response_code(403);
            die();
        }

        $role = $this->_usersService->validateUser($this->getParam('username'), $this->getParam('password'));

        $result->statusLogin = true;
        $result->role = $role;
        return $result;
    }
}

