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
        $username = $_GET['username'];
        $email = $_GET['email'];
        $password = $_GET['password'];

        $registered = $this->_usersService->registerUser($username, $email, $password);

        if($registered){
            header("Location: /login", true);
            die();
        } else {
            header("Location: /register", true);
            die();
        }
    }
}
