<?php

namespace render\login;

use render\defaultBuild\BaseRenderBuilder;

class Validate extends BaseRenderBuilder
{
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        if($_GET['username'] == "" || $_GET['password'] == ""){
            http_response_code(403);
            die();
        }
        if($this->_usersService->isLoggedInUser($this->getParam('cookie'))){
            header("Location: /", true);
            die();
        }
        $role = $this->_usersService->validateUser($_GET['username'], $_GET['password']);
        if($role == 9){
            header("Location: /admin", true);
            die();
        } elseif ($role != 9){
            header("Location: /", true);
            die();
        }
    }
}
