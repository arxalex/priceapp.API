<?php

namespace render\login;

use render\defaultBuild\BaseRenderBuilder;

class ValidateRegister extends BaseRenderBuilder
{
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        if($_GET['username'] == "" || $_GET['email'] == "" || $_GET['password'] == ""){
            http_response_code(403);
            die();
        }

        if($this->_usersService->isLoggedInUser($this->getParam('cookie'))){
            header("Location: /", true);
            die();
        }
        
        $username = $_GET['username'];
        $email = $_GET['email'];
        $password = $_GET['password'];

        $registered = $this->_usersService->registerUser($username, $email, $password);

        if($registered){
            header("Location: /register/confirm_email", true);
            die();
        } else {
            header("Location: /register", true);
            die();
        }
    }
}
