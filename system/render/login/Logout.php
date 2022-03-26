<?php

namespace render\login;

use render\defaultBuild\BaseRenderBuilder;

class Logout extends BaseRenderBuilder
{
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRedirect($this->getParam('cookie'), 1);
        $this->_usersService->logoutUser($this->getParam('cookie'));

        header("Location: /login", true);
        die();
    }
}
