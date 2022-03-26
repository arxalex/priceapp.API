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
        $this->_usersService->unavaliableRedirect($this->getParam('cookie'), 1);
        return '<account_change></account_change>';
    }
}
