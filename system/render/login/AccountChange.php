<?php

namespace render\login;

use render\defaultBuild\BaseRenderBuilder;

class AccountChange extends BaseRenderBuilder
{
    public function getExternalParams()
    {
        return [
            'scripts' => [
                '<script src="/system/scripts/login/accountChange.js"></script>'
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
        if(!$this->_usersService->isLoggedInUser($this->getParam('cookie'))){
            header("Location: /login", true);
            die();
        }
        return '<account_change></account_change>';
    }
}
