<?php

namespace render\admin;

use render\defaultBuild\BaseRenderBuilder;

class Admin extends BaseRenderBuilder
{
    public function getExternalParams()
    {
        return [
            'scripts' => [
                '<script src="/system/scripts/admin/admin.js"></script>'
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
        $this->_usersService->unavaliableRedirect($this->getParam('cookie'), 9);

        return '<admin></admin>';
    }
}
