<?php

namespace render\login;

use render\defaultBuild\BaseRenderBuilder;

class Login extends BaseRenderBuilder
{
    public function defaultParams()
    {
        return [
            'scripts' => [
                '<script src="/system/scripts/login/login.js"></script>',
            ],
            'links' => [
                '<link href="/system/style/login/login.css" rel="stylesheet">'
            ],
            'cookie' => []
        ];
    }
    public function build()
    {
        if($this->_usersService->isAdmin($this->getParam('cookie'))){
            header("Location: /admin", true);
            die();
        }
        return '<login></login>';
    }
}
