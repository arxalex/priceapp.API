<?php

namespace render\login;

use render\defaultBuild\BaseRenderBuilder;

class Register extends BaseRenderBuilder
{
    public function getExternalParams()
    {
        return [
            'scripts' => [
                '<script src="/system/scripts/login/register.js"></script>'
            ]
        ];
    }
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        if($this->_usersService->isLoggedInUser($this->getParam('cookie'))){
            header("Location: /", true);
            die();
        }
        return '<register></register>';
    }
}
