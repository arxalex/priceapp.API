Vue.component('categoryInsert', {
    template: `
        <div>
            <table class="table word-break">
                <tbody>
                    <tr>
                        <th>
                            <span>{{ sourceCategory }}</span>
                        </th>
                        <td>
                            <span>{{ sourceItem.image }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.image">
                        </td>
                    </tr>
                    
                </tbody>
            </table>
            <button class="btn btn-primary" v-on:click='insertCategory'></button>
        </div>
    `,
    data(){
        return {
            category: null,
            categoryLink: {
                id: null,
                categoryid: null,
                shopid: null,
                categoryshopid: null,
                shopcategorylabel: null
            }
        }
    },
    props: {
        sourceCategory: {
            type: String
        },
    },
    methods: {
        insertCategory: function() {
            
        },
        getItemsFromDb: function (url, data) {
            return axios.post(url, data).then((response) => {
                if (response.status == 200) {
                    return response.data;
                }
            });
        },
    },
    async mounted(){
        const cateoryLinkUrl = "../be/categories/get_categories";
        var categoryLink = await this.getItemsFromDb(cateoryLinkUrl, {
            method: "GetCategoryLinkByLabel",
            label: sourceCategory
        });
        this.categoryLink = categoryLink;
    },
    computed: {

    }
});

