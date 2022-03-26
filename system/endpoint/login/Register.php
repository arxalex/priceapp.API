<?php

namespace endpoint\login;

use endpoint\defaultBuild\BaseEndpointBuilder;
use stdClass;

class Register extends BaseEndpointBuilder
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
            'email' => '',
            'password' => ''
        ];
    }
    public function build()
    {
        $result = new stdClass();
        
        if($this->_usersService->isLoggedInUser($this->getParam('cookie'))){
            $result->statusRegister = false;
            return $result;
        }

        if($this->getParam('username') == "" || $this->getParam('email') == "" || $this->getParam('password') == ""){
            http_response_code(403);
            die();
        }

        $result->statusRegister = $this->_usersService->registerUser($this->getParam('username'), $this->getParam('email'), $this->getParam('password'));
        return $result;
    }
}
