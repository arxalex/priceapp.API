<?php

namespace render\admin;

use render\defaultBuild\BaseRenderBuilder;

class Admin extends BaseRenderBuilder
{
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRedirect($this->getParam('cookie'));

        return '
        <div class="container">
            <a class="btn btn-primary" href="/admin/items">Add items</a>
        </div>';
    }
}
