Vue.component('selectSearch', {
    template: `
        <div class="dropdown">
            <span class="dropdown-toggle form-control" id="dropdown" data-bs-toggle="dropdown" aria-expanded="false">{{ selectedLabel }}</span>
            <ul class="dropdown-menu" aria-labelledby="dropdown" style="max-height: 500px; overflow-y: scroll;">
                <input type="search" class="form-control dropdown-item" v-model="searchLabel">
                <li v-for="elem in elems" 
                    class="dropdown-item"
                    @click="selectedIdLocal = elem.id" 
                    :class="{ 'd-none': filter(elem.label, searchLabel)}">{{ elem.label }}
                </li>
            </ul>
        </div>
    `,
    data() {
        return {
            searchLabel: null,
            selectedLabel: "Select"
        }
    },
    props: {
        elems: {
            type: Array
        },
        selectedId: {
            type: Number
        }
    },
    model: {
        prop: 'selectedId',
        event: 'selectedIdСhange'
    },
    methods: {
        filter: function(shopLabel, search){
            return !(search == null || search.length == 0) && !(shopLabel.toLowerCase().includes(search.toLowerCase()));
        }
    },
    watch: {
        selectedIdLocal: function (value) {
            var temp = this.elems.filter(x => x.id == value);
            this.selectedLabel = temp.length == 1 ? temp[0].label : "Select";
        }
    },
    computed: {
        selectedIdLocal: {
            get: function() {
                return this.selectedId
            },
            set: function(value) {
                this.$emit('selectedIdСhange', value)
            }
        }
    },
    mounted(){
        var temp = this.elems.filter(x => x.id == this.selectedIdLocal);
        this.selectedLabel = temp.length == 1 ? temp[0].label : "Select";
    }
});

